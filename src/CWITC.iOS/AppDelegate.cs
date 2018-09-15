using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CWITC.Clients.UI;
using Xamarin.Forms;
using FormsToolkit.iOS;
using Xamarin.Forms.Platform.iOS;
using Xamarin;
using FormsToolkit;
using CWITC.Clients.Portable;
using Refractored.XamForms.PullToRefresh.iOS;
using Social;
using CoreSpotlight;
using CWITC.DataStore.Abstractions;
using System.Threading.Tasks;
using Xamarin.Auth;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace CWITC.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public static class ShortcutIdentifier
        {
            public const string Tweet = "org.cenwidev.cwitc.tweet";
            public const string Announcements = "org.cenwidev.cwitc.announcements";
            public const string Schedule = "org.cenwidev.cwitc.schedule";
            public const string Lunch = "org.cenwidev.cwitc.lunch";
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
			return false;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var openUrlOptions = new UIApplicationOpenUrlOptions(options);
            return OpenUrl(app, url, openUrlOptions.SourceApplication, options);
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var tint = UIColor.FromRGB(236, 47, 75);
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(47, 46, 46); //bar background
            UINavigationBar.Appearance.TintColor = tint; //Tint color of button items

            UIBarButtonItem.Appearance.TintColor = tint; //Tint color of button items

            UITabBar.Appearance.TintColor = tint;

            UISwitch.Appearance.OnTintColor = tint;

            UIAlertView.Appearance.TintColor = tint;

            UIView.AppearanceWhenContainedIn(typeof(UIAlertController)).TintColor = tint;
            UIView.AppearanceWhenContainedIn(typeof(UIActivityViewController)).TintColor = tint;
            UIView.AppearanceWhenContainedIn(typeof(SLComposeViewController)).TintColor = tint;

#if !DEBUG
            if (!string.IsNullOrWhiteSpace(ApiKeys.VSMobileCenterApiKeyIOS) && ApiKeys.VSMobileCenterApiKeyIOS != nameof(ApiKeys.VSMobileCenterApiKeyIOS)) 
            {
				MobileCenter
                    .Start(
                        ApiKeys.VSMobileCenterApiKeyIOS,
				        typeof(Analytics), 
                        typeof(Crashes));
            }
#endif

            Forms.Init();
            FormsMaps.Init();
            Toolkit.Init();

            DependencyService.Register<IAuthClient, Auth0Client>();

            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            SetMinimumBackgroundFetchInterval();

            //Random Inits for Linking out.
            Plugin.Share.ShareImplementation.ExcludedUIActivityTypes = new List<NSString>
            {
                UIActivityType.PostToFacebook,
                UIActivityType.AssignToContact,
                UIActivityType.OpenInIBooks,
                UIActivityType.PostToVimeo,
                UIActivityType.PostToFlickr,
                UIActivityType.SaveToCameraRoll
            };
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            NonScrollableListViewRenderer.Initialize();
            SelectedTabPageRenderer.Initialize();
            TextViewValue1Renderer.Init();
            PullToRefreshLayoutRenderer.Init();
            Syncfusion.SfRating.XForms.iOS.SfRatingRenderer.Init();

            LoadApplication(new App());

            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, DidBecomeActive);

			// This method verifies if you have been logged into the app before, and keep you logged in after you reopen or kill your app.
			//bool valid = Facebook.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);
			// 
			return  base.FinishedLaunching(app, options);
        }

        void DidBecomeActive(NSNotification notification)
        {
            ((CWITC.Clients.UI.App)Xamarin.Forms.Application.Current).SecondOnResume();

        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);
            ((CWITC.Clients.UI.App)Xamarin.Forms.Application.Current).SecondOnResume();
        }

#region Quick Action

        public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("OnActivated");

            // Handle any shortcut item being selected
            HandleShortcutItem(LaunchedShortcutItem);

            // Clear shortcut after it's been handled
            LaunchedShortcutItem = null;
        }

        // if app is already running
        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            Console.WriteLine("PerformActionForShortcutItem");
            // Perform action
            var handled = HandleShortcutItem(shortcutItem);
            completionHandler(handled);
        }

        public bool HandleShortcutItem(UIApplicationShortcutItem shortcutItem)
        {
            Console.WriteLine("HandleShortcutItem ");
            var handled = false;

            // Anything to process?
            if (shortcutItem == null)
                return false;

            // Take action based on the shortcut type
            switch (shortcutItem.Type)
            {
                case ShortcutIdentifier.Tweet:
                    Console.WriteLine("QUICKACTION: Tweet");
                    var slComposer = SLComposeViewController.FromService(SLServiceType.Twitter);
                    if (slComposer == null)
                    {
                        new UIAlertView("Unavailable", "Twitter is not available, please sign in on your devices settings screen.", null, "OK").Show();
                    }
                    else
                    {
                        slComposer.SetInitialText("#CWITC");
                        if (slComposer.EditButtonItem != null)
                        {
                            slComposer.EditButtonItem.TintColor = UIColor.FromRGB(118, 53, 235);
                        }
                        slComposer.CompletionHandler += (result) =>
                        {
                            InvokeOnMainThread(() => UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null));
                        };
                        
                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewControllerAsync(slComposer, true);
                    }
                    handled = true;
                    break;
                case ShortcutIdentifier.Announcements:
                    Console.WriteLine("QUICKACTION: Accouncements");
                    ContinueNavigation(AppPage.Notification);
                    handled = true;
                    break;
                case ShortcutIdentifier.Schedule:
                    Console.WriteLine("QUICKACTION: Schedule");
                    ContinueNavigation(AppPage.Schedule);
                    handled = true;
                    break;
                case ShortcutIdentifier.Lunch:
					Console.WriteLine("QUICKACTION: Lunhc Locations");
                    ContinueNavigation(AppPage.LunchLocations);
					handled = true;
					break;
            }

            Console.Write(handled);
            // Return results
            return handled;
        }

        void ContinueNavigation(AppPage page, string id = null)
        {
            Console.WriteLine("ContinueNavigation");

            // TODO: display UI in Forms somehow
            System.Console.WriteLine("Show the page for " + page);
            MessagingService.Current.SendMessage("DeepLinkPage", new DeepLinkPage
            {
                Page = page,
                Id = id
            });
        }

#endregion

#region Background Refresh

        private void SetMinimumBackgroundFetchInterval()
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(MINIMUM_BACKGROUND_FETCH_INTERVAL);
        }

        // Minimum number of seconds between a background refresh this is shorter than Android because it is easily killed off.
        // 20 minutes = 20 * 60 = 1200 seconds
        private const double MINIMUM_BACKGROUND_FETCH_INTERVAL = 1200;

        // Called whenever your app performs a background fetch
        public override async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Do Background Fetch
            var downloadSuccessful = false;
            try
            {
                Xamarin.Forms.Forms.Init();//need for dependency services
                // Download data
                var manager = DependencyService.Get <IStoreManager>();

                downloadSuccessful = await manager.SyncAllAsync(Settings.Current.IsLoggedIn);
            }
            catch (Exception ex)
            {
                var logger = DependencyService.Get <ILogger>();
                ex.Data["Method"] = "PerformFetch";
                logger.Report(ex);
            }

            // If you don't call this, your application will be terminated by the OS.
            // Allows OS to collect stats like data cost and power consumption
            if (downloadSuccessful)
            {
                completionHandler(UIBackgroundFetchResult.NewData);
                Settings.Current.HasSyncedData = true;
                Settings.Current.LastSync = DateTime.UtcNow;
            }
            else
            {
                completionHandler(UIBackgroundFetchResult.Failed);
            }
        }

#endregion

    }
}

