
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
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
using Android.Gms.Tasks;
using Firebase;

namespace CWITC.Droid
{


	[Activity(Name = "org.cenwidev.cwitc.MainActivity",
		Exported = true,
		Icon = "@drawable/ic_launcher",
		LaunchMode = LaunchMode.SingleTask,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
	    ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	[IntentFilter(new[] { Intent.ActionView },
			Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
			DataScheme = "org.cenwidev.cwitc",
			DataHost = "cwitc.auth0.com",
			DataPathPrefix = "/android/org.cenwidev.cwitc/callback")]
	public class MainActivity : FormsAppCompatActivity
	{
		const int RC_SIGN_IN = 9001;
		private TaskCompletionSource<GoogleSignInAccount> googleSignInTask;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			ToolbarResource = Resource.Layout.toolbar;
			TabLayoutResource = Resource.Layout.tabs;

			base.OnCreate(savedInstanceState);

			FirebaseApp.InitializeApp(this.ApplicationContext);

			Forms.Init(this, savedInstanceState);
			FormsMaps.Init(this, savedInstanceState);
			Toolkit.Init();
			global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);

			PullToRefreshLayoutRenderer.Init();
			typeof(Color).GetProperty("Accent", BindingFlags.Public | BindingFlags.Static).SetValue(null, Color.FromHex("#757575"));

			ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
			
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
				//CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
			}
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);

		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
		
		public void GoogleSignIn(GoogleApiClient apiClient, TaskCompletionSource<GoogleSignInAccount> tcs)
		{
			this.googleSignInTask = tcs;

			Intent signInIntent = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInIntent(apiClient);
			StartActivityForResult(signInIntent, RC_SIGN_IN);
		}

		public void OnComplete(Android.Gms.Tasks.Task task)
		{
			//if (task.IsSuccessful)
			//{
			//	bool isFetched = FirebaseRemoteConfig.Instance.ActivateFetched();
			//}
			//else
			//{

			//}
		}

		async void InitializeFirebase()
		{
			//FirebaseRemoteConfig.Instance
			//					.Fetch()
			//					.AddOnCompleteListener(this, this);
		}

	}
}