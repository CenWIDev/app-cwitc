
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
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Analytics;
using Firebase.RemoteConfig;
using Android.Gms.Tasks;

namespace CWITC.Droid
{


    [Activity(Label = "@string/app_name",
        Name = "org.cenwidev.cwitc.MainActivity",
        Exported = true,
        Icon = "@drawable/ic_launcher",
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity, Android.Gms.Tasks.IOnCompleteListener
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
                            Page = AppPage.Schedule
                        });
                        break;
                }
            }

            DataRefreshService.ScheduleRefresh(this);

            InitializeFirebase();
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

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsSuccessful)
            {
                bool isFetched = FirebaseRemoteConfig.Instance.ActivateFetched();
            }
            else
            {

            }

            Settings.Current.TwitterApiKey = FirebaseRemoteConfig.Instance.GetString("twitter_api_key");
            Settings.Current.TwitterApiSecret = FirebaseRemoteConfig.Instance.GetString("twitter_api_secret");
            Settings.Current.GrouveEventCode = FirebaseRemoteConfig.Instance.GetString("grouve_event_code");

            MessagingService.Current.SendMessage(MessageKeys.TwitterAuthRefreshed);
        }

        async void InitializeFirebase()
        {
            FirebaseRemoteConfig.Instance.SetDefaults(new Dictionary<string, Java.Lang.Object>
            {
                { "grouve_event_code", ApiKeys.GrouveEventCode }
            });

            FirebaseRemoteConfig.Instance
                                .Fetch()
                                .AddOnCompleteListener(this, this);
        }
    }
}

