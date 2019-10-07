using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FormsToolkit;
using MvvmHelpers;
using Xamarin.Forms;
using Plugin.Share;
using Plugin.Connectivity;

namespace CWITC.Clients.Portable
{
	public class AboutViewModel : SettingsViewModel
	{
		public ObservableRangeCollection<Grouping<string, MenuItem>> MenuItems { get; }
		public ObservableRangeCollection<MenuItem> InfoItems { get; } = new ObservableRangeCollection<MenuItem>();
		public ObservableRangeCollection<MenuItem> AccountItems { get; } = new ObservableRangeCollection<MenuItem>();

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

		MenuItem syncItem;
		MenuItem accountItem;
		public AboutViewModel()
		{
			AboutItems.Clear();

			InfoItems.AddRange(new[]
				{
					new MenuItem { Name = "About this app", Icon = "icon_venue.png", Parameter ="about" },
					new MenuItem { Name = "Sponsors", Icon = "icon_venue.png", Parameter="sponsors"},
					new MenuItem { Name = "Event Map", Icon = "icon_venue.png", Parameter = "floor-maps"},
                    //new MenuItem { Name = "Lunch Locations", Icon = "ic_restaurant_menu.png", Parameter = "lunch-locations"},
                    new MenuItem { Name = "Venue", Icon = "icon_venue.png", Parameter = "venue"},
				});

			accountItem = new MenuItem
			{
				Name = "Logged in as:"
			};

			syncItem = new MenuItem
			{
				Name = "Last Sync:"
			};

			UpdateItems();

			AccountItems.Add(accountItem);
			AccountItems.Add(syncItem);

			//This will be triggered wen 
			Settings.PropertyChanged += async (sender, e) =>
				{
					if (e.PropertyName == "Email")
					{
						Settings.NeedsSync = true;

						UpdateItems();

						OnPropertyChanged("LoginText");
						OnPropertyChanged("AccountItems");
						//if logged in you should go ahead and sync data.
						if (Settings.IsLoggedIn)
						{
							await ExecuteSyncCommandAsync();
						}
					}

					if (e.PropertyName == "Email" || e.PropertyName == "LastSync" || e.PropertyName == "PushNotificationsEnabled")
					{

						OnPropertyChanged("AccountItems");
					}
				};
		}

		public void UpdateItems()
		{
			syncItem.Subtitle = LastSyncDisplay;
			accountItem.Subtitle = Settings.Current.IsLoggedIn ? Settings.Current.UserDisplayName : "Not signed in";
		}
	}
}

