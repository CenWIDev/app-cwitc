
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

		protected override void OnCreate(Bundle savedInstanceState)
		{
			ToolbarResource = Resource.Layout.toolbar;
			TabLayoutResource = Resource.Layout.tabs;

			base.OnCreate(savedInstanceState);

			Forms.Init(this, savedInstanceState);
			FormsMaps.Init(this, savedInstanceState);
			Toolkit.Init();

			DependencyService.Register<IAuthClient, Auth0Client>();

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
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);

			Auth0.OidcClient.ActivityMediator.Instance.Send(intent.DataString);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}