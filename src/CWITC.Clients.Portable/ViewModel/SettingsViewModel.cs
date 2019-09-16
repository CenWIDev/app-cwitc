using System;
using System.Windows.Input;
using System.Threading.Tasks;
using FormsToolkit;
using Plugin.Connectivity;
using Plugin.Share;
using Xamarin.Forms;
using MvvmHelpers;
using Humanizer;

namespace CWITC.Clients.Portable
{
	public class SettingsViewModel : ViewModelBase
	{
		public ObservableRangeCollection<MenuItem> AboutItems { get; } = new ObservableRangeCollection<MenuItem>();

		public SettingsViewModel()
		{
			//This will be triggered wen 
			Settings.PropertyChanged += async (sender, e) =>
			{
				if (e.PropertyName == "Email")
				{
					Settings.NeedsSync = true;
					OnPropertyChanged("LoginText");
					OnPropertyChanged("AccountItems");
					//if logged in you should go ahead and sync data.
					if (Settings.IsLoggedIn)
					{
						await ExecuteSyncCommandAsync();
					}
				}
			};

			AboutItems.AddRange(new[]
				{
					new MenuItem { Name = "Created by CENWIDEV with <3", Command=LaunchBrowserCommand, Parameter="http://cenwidev.org" },
					new MenuItem { Name = "Open source on GitHub!", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc"},
					new MenuItem { Name = "Terms of Use", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc/wiki/Terms-&-Conditions"},
					new MenuItem { Name = "Privacy Policy", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc/wiki/Privacy-Policy"},
					new MenuItem { Name = "Code of Conduct", Command=LaunchBrowserCommand, Parameter="https://cwitc.org/code-of-conduct"},
					new MenuItem { Name = "Open Source Notice", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc/tree/master/oss-licenses"}
				});
		}

		public string LastSyncDisplay
		{
			get
			{
				if (!Settings.HasSyncedData)
					return "Never";

				return Settings.LastSync.Humanize();
			}
		}

		ICommand logoutCommand;
		public ICommand LogoutCommand =>
			logoutCommand ?? (logoutCommand = new Command(async () => await ExecuteLogoutCommand()));

		async Task ExecuteLogoutCommand()
		{

			if (!CrossConnectivity.Current.IsConnected)
			{
				MessagingUtils.SendOfflineMessage();
				return;
			}


			if (IsBusy)
				return;

			Logger.Track(EvolveLoggerKeys.Logout);

			try
			{
				var ssoClient = DependencyService.Get<ISSOClient>();
				if (ssoClient != null)
				{
					await ssoClient.LogoutAsync();
				}

				// clear Log In data first!
				Settings.ClearUserData();

				MessagingService.Current.SendMessage(MessageKeys.LoggedOut);
			}
			catch (Exception ex)
			{
				ex.Data["method"] = "ExecuteLoginCommandAsync";
				//TODO validate here.
				Logger.Report(ex);
			}
		}

		string syncText = "Sync Now";
		public string SyncText
		{
			get { return syncText; }
			set { SetProperty(ref syncText, value); }
		}

		ICommand syncCommand;
		public ICommand SyncCommand =>
			syncCommand ?? (syncCommand = new Command(async () => await ExecuteSyncCommandAsync()));

		async Task ExecuteSyncCommandAsync()
		{

			if (IsBusy)
				return;

			if (!CrossConnectivity.Current.IsConnected)
			{
				MessagingUtils.SendOfflineMessage();
				return;
			}

			Logger.Track(EvolveLoggerKeys.ManualSync);

			SyncText = "Syncing...";
			IsBusy = true;

			try
			{
#if DEBUG
				await Task.Delay(1000);
#endif

				Settings.HasSyncedData = true;
				Settings.LastSync = DateTime.UtcNow;
				OnPropertyChanged("LastSyncDisplay");

				await StoreManager.SyncAllAsync(Settings.Current.IsLoggedIn);
				if (!Settings.Current.IsLoggedIn)
				{
					MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
					{
						Title = "CWITC Data Synced",
						Message = "You now have the latest conference data, however to sync your favorites and feedback you must Log In.",
						Cancel = "OK"
					});
				}

			}
			catch (Exception ex)
			{
				ex.Data["method"] = "ExecuteSyncCommandAsync";
				MessagingUtils.SendAlert("Unable to sync", "Uh oh, something went wrong with the sync, please try again. \n\n Debug:" + ex.Message);
				Logger.Report(ex);
			}
			finally
			{
				SyncText = "Sync Now";
				IsBusy = false;
			}
		}

		public string Copyright => $"Copyright {DateTime.Today.Year} - CENWIDEV.";
	}
}

