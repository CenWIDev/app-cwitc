
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
using Android.Gms.Auth.Api.SignIn;
//using Gcm;
//using Gcm.Client;

namespace CWITC.Droid
{


    [Activity(Label = "@string/app_name",
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
        const int RC_SIGN_IN = 9001;
        TaskCompletionSource<GoogleSignInAccount> googleSignInTask = null;

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

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == RC_SIGN_IN)
            {
                var result = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (result.IsSuccess)
                {
                    //result.SignInAccount.
                    googleSignInTask.SetResult(result.SignInAccount);
                }
                else
                {
                    googleSignInTask.SetCanceled();
                }   
            }
            else
            {
                CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            }
        }

        void InitializeHockeyApp()
        {
            if (string.IsNullOrWhiteSpace(ApiKeys.HockeyAppAndroid) || ApiKeys.HockeyAppAndroid == nameof(ApiKeys.HockeyAppAndroid))
                return;


            HockeyApp.Android.CrashManager.Register(this, ApiKeys.HockeyAppAndroid);
            //HockeyApp.Android.UpdateManager.Register(this, ApiKeys.HockeyAppAndroid);

            HockeyApp.Android.Metrics.MetricsManager.Register(Application, ApiKeys.HockeyAppAndroid);

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
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void GoogleSignIn(GoogleApiClient apiClient, TaskCompletionSource<GoogleSignInAccount> tcs)
        {
            this.googleSignInTask = tcs;

            Intent signInIntent = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInIntent(apiClient);
			StartActivityForResult(signInIntent, RC_SIGN_IN);
        }
    }
}

