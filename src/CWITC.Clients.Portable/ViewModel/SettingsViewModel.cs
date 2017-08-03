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
        public ObservableRangeCollection<MenuItem> TechnologyItems { get; } = new ObservableRangeCollection<MenuItem>();

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
                    new MenuItem { Name = "Terms of Use", Command=LaunchBrowserCommand, Parameter="https://cwitc.org"},
                    new MenuItem { Name = "Privacy Policy", Command=LaunchBrowserCommand, Parameter="https://cwitc.org"},
                    new MenuItem { Name = "Open Source Notice", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc/tree/master/oss-licenses"}
                });

            TechnologyItems.AddRange(new[]
                {
                    new MenuItem { Name = "Azure Mobile Apps", Command=LaunchBrowserCommand, Parameter="https://github.com/Azure/azure-mobile-apps-net-client/" },
                    new MenuItem { Name = "Censored", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Censored"},
                    new MenuItem { Name = "Calendar Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/TheAlmightyBob/Calendars"},
                    new MenuItem { Name = "Connectivity Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Connectivity"},
                    new MenuItem { Name = "Embedded Resource Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/JosephHill/EmbeddedResourcePlugin"},
                    new MenuItem { Name = "External Maps Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/ExternalMaps"},
                    new MenuItem { Name = "Humanizer", Command=LaunchBrowserCommand, Parameter="https://github.com/Humanizr/Humanizer"},
                    new MenuItem { Name = "Image Circles", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/ImageCircle"},
                    new MenuItem { Name = "Json.NET", Command=LaunchBrowserCommand, Parameter="https://github.com/JamesNK/Newtonsoft.Json"},
                    new MenuItem { Name = "LinqToTwitter", Command=LaunchBrowserCommand, Parameter="https://github.com/JoeMayo/LinqToTwitter"},
                    new MenuItem { Name = "Messaging Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/cjlotz/Xamarin.Plugins"},
                    new MenuItem { Name = "Mvvm Helpers", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/mvvm-helpers"},
                    new MenuItem { Name = "Noda Time", Command=LaunchBrowserCommand, Parameter="https://github.com/nodatime/nodatime"},
                    new MenuItem { Name = "Permissions Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Permissions"},
                    new MenuItem { Name = "PCL Storage", Command=LaunchBrowserCommand, Parameter="https://github.com/dsplaisted/PCLStorage"},
                    new MenuItem { Name = "Pull to Refresh Layout", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Forms-PullToRefreshLayout"},
                    new MenuItem { Name = "Settings Plugin", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Settings"},
                    new MenuItem { Name = "Toolkit for Xamarin.Forms", Command=LaunchBrowserCommand, Parameter="https://github.com/jamesmontemagno/xamarin.forms-toolkit"},
                    new MenuItem { Name = "Xamarin.Forms", Command=LaunchBrowserCommand, Parameter="http://xamarin.com/forms"},
                    new MenuItem { Name = "Xamarin Insights", Command=LaunchBrowserCommand, Parameter="http://xamarin.com/insights"},
                    new MenuItem { Name = "ZXing.Net Mobile", Command=LaunchBrowserCommand, Parameter="https://github.com/Redth/ZXing.Net.Mobile"}
                    });
        }

        public string LoginText => Settings.IsLoggedIn ? "Sign Out" : "Sign In";

        public string LastSyncDisplay
        {
            get
            {
                if (!Settings.HasSyncedData)
                    return "Never";

                return Settings.LastSync.Humanize();
            }
        }

        ICommand loginCommand;
        public ICommand LoginCommand =>
            loginCommand ?? (loginCommand = new Command(ExecuteLoginCommand));

        void ExecuteLoginCommand()
        {

            if (!CrossConnectivity.Current.IsConnected)
            {
                MessagingUtils.SendOfflineMessage();
                return;
            }


            if (IsBusy)
                return;


            if (Settings.IsLoggedIn)
            {

                MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.Question, new MessagingServiceQuestion
                {
                    Title = "Logout?",
                    Question = "Are you sure you want to logout? You can only save favorites and leave feedback when logged in.",
                    Positive = "Yes, Logout",
                    Negative = "Cancel",
                    OnCompleted = async (result) =>
                        {
                            if (!result)
                                return;

                            await Logout();
                        }
                });

                return;
            }

            Logger.TrackPage(AppPage.Login.ToString(), "Settings");
            MessagingService.Current.SendMessage(MessageKeys.NavigateLogin);

        }

        async Task Logout()
        {
            Logger.Track(EvolveLoggerKeys.Logout);

            try
            {
                ISSOClient ssoClient = DependencyService.Get<ISSOClient>();
                if (ssoClient != null)
                {
                    await ssoClient.LogoutAsync();
                }

                // clear sign in data first!
                Settings.FirstName = string.Empty;
                Settings.LastName = string.Empty;
                Settings.Email = string.Empty; //this triggers login text changed!

                // then log back in as an anonymous user.
                await DoAnonymousLogin();

                await ExecuteSyncCommandAsync();
            }
            catch (Exception ex)
            {
                ex.Data["method"] = "ExecuteLoginCommandAsync";
                //TODO validate here.
                Logger.Report(ex);
            }
        }

        async Task DoAnonymousLogin()
        {
            ISSOClient client = DependencyService.Get<ISSOClient>();
            AccountResponse result = await client.LoginAnonymously();

            if (result?.Success ?? false)
            {
                try
                {
                    IsBusy = true;
                    Settings.UserId = result.User?.Id;
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
                    IsBusy = false;
                }

                Settings.FirstRun = false;
            }
            else
            {
                Logger.Track(EvolveLoggerKeys.LoginFailure, "Reason", result.Error);
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Anonymous Sign In Failed",
                    Message = result.Error,
                    Cancel = "OK"
                });
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
                        Message = "You now have the latest conference data, however to sync your favorites and feedback you must sign in.",
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
    }
}

