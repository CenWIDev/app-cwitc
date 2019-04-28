using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;
using Plugin.Share;
using FormsToolkit;
using Plugin.Share.Abstractions;

namespace CWITC.Clients.Portable
{
    public class LoginViewModel : ViewModelBase
    {
        ISSOClient client;
        public LoginViewModel(INavigation navigation) : base(navigation)
        {
            client = DependencyService.Get<ISSOClient>();
        }

        string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        ICommand facebookLoginCommand;
        public ICommand FacebookLoginCommand =>
        facebookLoginCommand ?? (facebookLoginCommand = new Command(async () => await ExecuteFacebookLoginAsync()));

        async Task ExecuteFacebookLoginAsync()
        {
            await LoginForProvider("Facebook", () => client.LoginWithFacebook());
        }

		ICommand googleLoginCommand;
		public ICommand GoogleLoginCommand =>
		googleLoginCommand ?? (googleLoginCommand = new Command(async () => await ExecuteGoogleLoginAsync()));

		async Task ExecuteGoogleLoginAsync()
		{
            await LoginForProvider("Google", () => client.LoginWithGoogle());
		}

		ICommand twitterLoginCommand;
		public ICommand TwitterLoginCommand =>
		twitterLoginCommand ?? (twitterLoginCommand = new Command(async () => await ExecuteTwitterLoginAsync()));

		async Task ExecuteTwitterLoginAsync()
		{
			await LoginForProvider("Twitter", () => client.LoginWithTwitter());
		}

		ICommand githubLoginCommand;
		public ICommand GitHubLoginCommand =>
		githubLoginCommand ?? (githubLoginCommand = new Command(async () => await ExecuteGithubLoginAsync()));

		async Task ExecuteGithubLoginAsync()
		{
			await LoginForProvider("GitHub", () => client.LoginWithGithub());
		}

        async Task LoginForProvider(string providerName, Func<Task<AccountResponse>> providerLogin)
		{
			if (IsBusy)
				return;

			try
			{
				IsBusy = true;
				Message = "Signing in...";
#if DEBUG
				await Task.Delay(1000);
#endif
				AccountResponse result = await providerLogin();

				if (result?.Success ?? false)
				{
					Message = "Updating schedule...";
					Settings.FirstName = result.User?.FirstName ?? string.Empty;
					Settings.LastName = result.User?.LastName ?? string.Empty;
					Settings.Email = result.User?.Email.ToLowerInvariant();
					Settings.UserId = result.User?.Id;

					Logger.Track(EvolveLoggerKeys.LoginSuccess);
					try
					{
						await StoreManager.SyncAllAsync(true);
						Settings.Current.LastSync = DateTime.UtcNow;
						Settings.Current.HasSyncedData = true;
					}
					catch (Exception ex)
					{
						//if sync doesn't work don't worry it is alright we can recover later
						Logger.Report(ex);
					}

					Settings.FirstRun = false;
                    Finish();
				}
				else
				{
					Logger.Track(EvolveLoggerKeys.LoginFailure, "Reason", result.Error);
					MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
					{
						Title = "Unable to Log In",
						Message = result.Error,
						Cancel = "OK"
					});
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				Logger.Track(EvolveLoggerKeys.LoginFailure, "Reason", ex?.Message ?? string.Empty);

				MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
				{
					Title = "Unable to Log In",
                    Message = $"{providerName} Log In Failed",
					Cancel = "OK"
				});
			}
			finally
			{
				Message = string.Empty;
				IsBusy = false;
			}
		}

        void Finish()
        {
			MessagingService.Current.SendMessage(MessageKeys.LoggedIn);
		}
    }
}