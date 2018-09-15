using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;
using Plugin.Share;
using FormsToolkit;
using Plugin.Share.Abstractions;
using Newtonsoft.Json;

namespace CWITC.Clients.Portable
{
	public class LoginViewModel : ViewModelBase
	{
		IAuthClient client;
		public LoginViewModel(INavigation navigation) : base(navigation)
		{
			client = DependencyService.Get<IAuthClient>();
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
			await LoginForProvider("Facebook", () => client.LoginAsync());
		}

		ICommand googleLoginCommand;
		public ICommand GoogleLoginCommand =>
		googleLoginCommand ?? (googleLoginCommand = new Command(async () => await ExecuteGoogleLoginAsync()));

		async Task ExecuteGoogleLoginAsync()
		{
			await LoginForProvider("Google", () => client.LoginAsync());
		}

		ICommand cancelCommand;
		public ICommand CancelCommand =>
			cancelCommand ?? (cancelCommand = new Command(async () => await ExecuteCancelAsync()));

		async Task ExecuteCancelAsync()
		{
			Logger.Track(EvolveLoggerKeys.LoginCancel);
			if (Settings.FirstRun)
			{
				try
				{
					MessagingService.Current.SendMessage(MessageKeys.LoggedIn);
					await StoreManager.SyncAllAsync(false);
					Settings.Current.LastSync = DateTime.UtcNow;
					Settings.Current.HasSyncedData = true;
				}
				catch (Exception ex)
				{
					//if sync doesn't work don't worry it is alright we can recover later
					Logger.Report(ex);
				}
				finally
				{
					Message = string.Empty;
					IsBusy = false;
				}

				Settings.FirstRun = false;
				await Finish();
			}

			else
			{
				await Finish();

			}
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

					Settings.UserId = result.User?.Id;
					Settings.FirstName = result.User?.FirstName ?? string.Empty;
					Settings.LastName = result.User?.LastName ?? string.Empty;
					Settings.Email = (result.User?.Email ?? string.Empty).ToLowerInvariant();

					MessagingService.Current.SendMessage(MessageKeys.LoggedIn);
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
					await Finish();
				}
				else
				{
					Logger.Track(EvolveLoggerKeys.LoginFailure, "Reason", result.Error);
					MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
					{
						Title = "Unable to Sign in",
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
					Title = "Unable to Sign in",
					Message = $"{providerName} Sign In Failed",
					Cancel = "OK"
				});
			}
			finally
			{
				Message = string.Empty;
				IsBusy = false;
			}
		}

		async Task Finish()
		{
			if (Device.OS == TargetPlatform.iOS && Settings.FirstRun)
			{
				await Navigation.PopModalAsync();
			}
			else
			{
				await Navigation.PopModalAsync();
			}
		}
	}
}