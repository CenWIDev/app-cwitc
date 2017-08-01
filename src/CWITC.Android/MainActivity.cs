
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.AppIndexing;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Gcm;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using FormsToolkit;
using FormsToolkit.Droid;
using Plugin.Permissions;
using Refractored.XamForms.PullToRefresh.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CWITC.Clients.Portable;
using CWITC.Clients.UI;
using CWITC.DataObjects;
using Xamarin;
//using Gcm;
//using Gcm.Client;
using CWITC.Droid.Notifications;

namespace CWITC.Droid
{


    [Activity(Label = "CWITC 17",
        Name = "org.cenwidev.cwitc.MainActivity",
        Exported = true,
        Icon = "@drawable/ic_launcher",
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "http",
        DataPathPrefix = "/session/",
        DataHost = "cwitc.org")]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "https",
        DataPathPrefix = "/session/",
        DataHost = "cwitc.org")]

    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "http",
        DataHost = "cwitc.org")]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "https",
        DataHost = "cwitc.org")]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "@PACKAGE_NAME@",
        DataHost = "cwitc.auth0.com",
        DataPathPrefix = "/android/@PACKAGE_NAME@/callback")]
	[IntentFilter(new[] { Intent.ActionView },
		Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
		DataScheme = "@PACKAGE_NAME@",
		DataHost = "cwitc.auth0.com",
		DataPathPrefix = "/android/@PACKAGE_NAME@/logout")]
    public class MainActivity : FormsAppCompatActivity
    {
        public Xamarin.Facebook.ICallbackManager CallbackManager { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CallbackManager = Xamarin.Facebook.CallbackManagerFactory.Create();

            ToolbarResource = Resource.Layout.toolbar;
            TabLayoutResource = Resource.Layout.tabs;

            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);
            FormsMaps.Init(this, savedInstanceState);
            Toolkit.Init();

            DependencyService.Register<ISSOClient, AndroidAuthSSOClient>();

            PullToRefreshLayoutRenderer.Init();
            typeof(Color).GetProperty("Accent", BindingFlags.Public | BindingFlags.Static).SetValue(null, Color.FromHex("#757575"));

            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();

            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            InitializeHockeyApp();

            LoadApplication(new App());

            var gpsAvailable = IsPlayServicesAvailable();
            Settings.Current.PushNotificationsEnabled = gpsAvailable;

            OnNewIntent(Intent);


            if (!string.IsNullOrWhiteSpace(Intent?.Data?.LastPathSegment))
            {

                switch (Intent.Data.LastPathSegment)
                {
                    case "sessions":
                        MessagingService.Current.SendMessage<DeepLinkPage>("DeepLinkPage", new DeepLinkPage
                        {
                            Page = AppPage.Sessions
                        });
                        break;
                    case "events":
                        MessagingService.Current.SendMessage<DeepLinkPage>("DeepLinkPage", new DeepLinkPage
                        {
                            Page = AppPage.Events
                        });
                        break;
                }
            }

            if (!Settings.Current.PushNotificationsEnabled)
                return;

            RegisterWithGCM();

            DataRefreshService.ScheduleRefresh(this);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            bool? isAuth0 = intent?.DataString?.Contains("auth0");
            bool? isLogout = intent.DataString?.Contains("logout");

            if (isAuth0.HasValue && isAuth0.Value)
            {
                if (isLogout.HasValue && isLogout.Value)
                    MessagingService.Current.SendMessage(MessageKeys.LogoutCallback);
                else
                    MessagingService.Current.SendMessage(MessageKeys.LoginCallback, intent.DataString);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        void InitializeHockeyApp()
        {
            if (string.IsNullOrWhiteSpace(ApiKeys.HockeyAppAndroid) || ApiKeys.HockeyAppAndroid == nameof(ApiKeys.HockeyAppAndroid))
                return;


            HockeyApp.Android.CrashManager.Register(this, ApiKeys.HockeyAppAndroid);
            //HockeyApp.Android.UpdateManager.Register(this, ApiKeys.HockeyAppAndroid);

            HockeyApp.Android.Metrics.MetricsManager.Register(Application, ApiKeys.HockeyAppAndroid);

        }

        private void RegisterWithGCM()
        {
            // Check to ensure everything's set up right
            //GcmClient.CheckDevice(this);
            //GcmClient.CheckManifest(this);

            EvolveRegistrationService.Register(this);

            // Register for push notifications
            //System.Diagnostics.Debug.WriteLine("MainActivity", "Registering...");
            //GcmService.Initialize(this);
            //GcmService.Register(this);
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    if (Settings.Current.GooglePlayChecked)
                        return false;

                    Settings.Current.GooglePlayChecked = true;
                    Toast.MakeText(this, "Google Play services is not installed, push notifications have been disabled.", ToastLength.Long).Show();
                }
                else
                {
                    Settings.Current.PushNotificationsEnabled = false;
                }
                return false;
            }
            else
            {
                Settings.Current.PushNotificationsEnabled = true;
                return true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}

