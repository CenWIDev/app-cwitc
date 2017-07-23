﻿using Xamarin.Forms;
using CWITC.DataStore.Abstractions;

using MvvmHelpers;
using Plugin.Share;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Share.Abstractions;
using System;

namespace CWITC.Clients.Portable
{
    public class ViewModelBase : BaseViewModel
    {
        protected INavigation Navigation { get; }

        public ViewModelBase(INavigation navigation = null)
        {
            Navigation = navigation;
        }

        public static void Init(bool mock = true)
        {
            if (mock)
            {
                //DependencyService.Register<ISessionStore, CWITC.DataStore.Mock.SessionStore> ();
                //DependencyService.Register<IFavoriteStore, CWITC.DataStore.Mock.FavoriteStore> ();
                //DependencyService.Register<IFeedbackStore, CWITC.DataStore.Mock.FeedbackStore> ();
                //DependencyService.Register<ISpeakerStore, CWITC.DataStore.Mock.SpeakerStore> ();
                //DependencyService.Register<ISponsorStore, CWITC.DataStore.Mock.SponsorStore> ();
                //DependencyService.Register<ICategoryStore, CWITC.DataStore.Mock.CategoryStore> ();
                //DependencyService.Register<IScheduleStore, CWITC.DataStore.Mock.EventStore> ();
                DependencyService.Register<INotificationStore, CWITC.DataStore.Mock.NotificationStore>();
                //DependencyService.Register<IStoreManager, CWITC.DataStore.Mock.StoreManager> ();
            }
            else
            {
                // todo: some other data store that is real

                //DependencyService.Register<ISessionStore, CWITC.DataStore.Azure.SessionStore> ();
                //DependencyService.Register<IFavoriteStore, CWITC.IOS.FavoriteStore> ();
                //DependencyService.Register<IFeedbackStore, CWITC.DataStore.Azure.FeedbackStore> ();
                //DependencyService.Register<ISpeakerStore, CWITC.DataStore.Azure.SpeakerStore> ();
                //DependencyService.Register<ISponsorStore, CWITC.DataStore.Azure.SponsorStore> ();
                //DependencyService.Register<ICategoryStore, CWITC.DataStore.Azure.CategoryStore> ();
                //DependencyService.Register<IEventStore, CWITC.DataStore.Azure.EventStore> ();
                //DependencyService.Register<INotificationStore, CWITC.DataStore.Azure.NotificationStore> ();
                //DependencyService.Register<ISSOClient, CWITC.Clients.Portable.Auth.Azure.XamarinSSOClient> ();
                //DependencyService.Register<IStoreManager, CWITC.DataStore.Azure.StoreManager> ();
            }
        }

        protected ILogger Logger { get; } = DependencyService.Get<ILogger>();
        protected IStoreManager StoreManager { get; } = DependencyService.Get<IStoreManager>();
        protected IToast Toast { get; } = DependencyService.Get<IToast>();

        protected FavoriteService FavoriteService { get; } = DependencyService.Get<FavoriteService>();


        public Settings Settings
        {
            get { return Settings.Current; }
        }

        ICommand launchBrowserCommand;
        public ICommand LaunchBrowserCommand =>
        launchBrowserCommand ?? (launchBrowserCommand = new Command<string>(async (t) => await ExecuteLaunchBrowserAsync(t)));

        async Task ExecuteLaunchBrowserAsync(string arg)
        {
            if (IsBusy)
                return;

            if (!arg.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !arg.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                arg = "http://" + arg;

            Logger.Track(EvolveLoggerKeys.LaunchedBrowser, "Url", arg);

            var lower = arg.ToLowerInvariant();
            if (Device.OS == TargetPlatform.iOS && lower.Contains("twitter.com"))
            {
                try
                {
                    var id = arg.Substring(lower.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    var launchTwitter = DependencyService.Get<ILaunchTwitter>();
                    if (lower.Contains("/status/"))
                    {
                        //status
                        if (launchTwitter.OpenStatus(id))
                            return;
                    }
                    else
                    {
                        //user
                        if (launchTwitter.OpenUserName(id))
                            return;
                    }
                }
                catch
                {
                }
            }

            try
            {
                await CrossShare.Current.OpenBrowser(arg, new BrowserOptions
                {
                    ChromeShowTitle = true,
                    ChromeToolbarColor = new ShareColor
                    {
                        A = 255,
                        R = 118,
                        G = 53,
                        B = 235
                    },
                    UseSafairReaderMode = true,
                    UseSafariWebViewController = true
                });
            }
            catch
            {
            }
        }



    }
}


