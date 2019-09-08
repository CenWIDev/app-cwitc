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
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Auth;

namespace CWITC.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        public static class ShortcutIdentifier
        {
            public const string Schedule = "org.cenwidev.cwitc.schedule";
            public const string Lunch = "org.cenwidev.cwitc.lunch";
        }

		public static Action<string> CallbackHandler { get; internal set; }

		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			return true;
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
			var openUrlOptions = new UIApplicationOpenUrlOptions(options);
			return OpenUrl(app, url, openUrlOptions.SourceApplication, options);
			////var openUrlOptions = new UIApplicationOpenUrlOptions(options);
			////return OpenUrl(app, url, openUrlOptions.SourceApplication, options);

			//var urlString = url.AbsoluteString;
			//if (urlString.Contains("login-callback"))
			//{
			//	// Convert NSUrl to Uri
			//	var uri = new Uri(url.AbsoluteString);
			//	// Load redirectUrl page
			//	AuthenticationState.Authenticator.OnPageLoading(uri);
			//}

			//// todo: other urls

			//return true;
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
			if (!string.IsNullOrWhiteSpace(ApiKeys.VSMobileCenterApiKeyIOS))
			{
				Microsoft.AppCenter.AppCenter
				         .Start(ApiKeys.VSMobileCenterApiKeyIOS,
					typeof(Analytics),
					typeof(Crashes));
			}
#endif
			Firebase.Core.App.Configure();

			Forms.Init();
            FormsMaps.Init();
            Toolkit.Init();
			global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();

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
                case ShortcutIdentifier.Schedule:
                    Console.WriteLine("QUICKACTION: Schedule");
                    ContinueNavigation(AppPage.Schedule);
                    handled = true;
                    break;
                case ShortcutIdentifier.Lunch:
					Console.WriteLine("QUICKACTION: Lunch Locations");
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

    }
}