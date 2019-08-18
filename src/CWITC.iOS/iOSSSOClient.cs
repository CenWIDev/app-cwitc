using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.Clients.UI;
using CWITC.iOS;
using Firebase.Auth;
using Foundation;
using Google.SignIn;
using IdentityModel.OidcClient;
using SafariServices;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(iOSAuthSSOClient))]
namespace CWITC.iOS
{
	public partial class iOSAuthSSOClient : NSObject, ISSOClient,
		ISFSafariViewControllerDelegate, ISignInDelegate, ISignInUIDelegate
	{
		static iOSAuthSSOClient()
		{
			var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
			Google.SignIn.SignIn.SharedInstance.ClientID = googleServiceDictionary["CLIENT_ID"].ToString();
		}

		#region Facebook Auth

		public async Task<AccountResponse> LoginWithFacebook()
		{
			//_facebookAuth = new OAuth2Authenticator(
			//		"1391179434308637",
			//		"email",
			//		authorizeUrl: new Uri("https://www.facebook.com/dialog/oauth/"),
			//		redirectUrl: new Uri("https://central-wi-it-conference.firebaseapp.com/__/auth/handler"),
			//		isUsingNativeUI: false);

			//_facebookAuth.Completed += OnOAuth2AuthCompleted;
			//_facebookAuth.Error += OnOAuth2AuthError;

			//var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
			//presenter.Login(_facebookAuth);

			//this.loginTCS = new TaskCompletionSource<Account>();
			//var account = await this.loginTCS.Task;

			//var accessToken = account.Properties["access_token"];

			//var credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
			//var firebaseResult = await LoginToFirebase(credential);
			//if (firebaseResult.Success)
			//{
			//	Settings.Current.AuthType = "facebook";
			//	//firebaseResult.User.Email = emailAddress;
			//}

			//return firebaseResult;
			try
			{
				var topVC = GetViewController();

				var fbLogin = new Facebook.LoginKit.LoginManager();

				var loginResult = fbLogin.LogInWithReadPermissionsAsync(new string[] { "public_profile email" }, topVC);

				while (loginResult.Status != TaskStatus.RanToCompletion &&
					loginResult.Status != TaskStatus.Faulted &&
					loginResult.Status != TaskStatus.Canceled)
				{
					await Task.Delay(TimeSpan.FromMilliseconds(500));
				}

				if (loginResult.IsCanceled)
				{
					return new AccountResponse
					{
						Success = false,
						Error = "Login Cancelled"
					};
				}

				var accessToken = Facebook.CoreKit.AccessToken.CurrentAccessToken;

				TaskCompletionSource<string> getEmailTask = new TaskCompletionSource<string>();
				// gets the email & name for this user
				var graphRequest = new Facebook.CoreKit.GraphRequest(
					"me",
					NSDictionary.FromObjectAndKey(new NSString("id,email"),
					new NSString("fields")));

				graphRequest.Start((connection, graphResult, error) =>
				{
					if (error != null)
					{
						NSObject message;
						error.UserInfo.TryGetValue(new NSString("com.facebook.sdk:FBSDKErrorDeveloperMessageKey"), out message);

						string exMessage = null;
						if (message != null)
							exMessage = message?.ToString();
						else
							exMessage = error.LocalizedDescription;

						throw new Exception(exMessage);
					}

					var email = (NSString)graphResult.ValueForKey(new NSString("email"));

					getEmailTask.SetResult(email);
				});

				string emailAddress = await getEmailTask.Task;

				// Get access token for the signed-in user and exchange it for a Firebase credential
				var credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken.TokenString);
				var firebaseResult = await LoginToFirebase(credential);
				if (firebaseResult.Success)
				{
					Settings.Current.AuthType = "facebook";
					firebaseResult.User.Email = emailAddress;
				}

				return firebaseResult;
			}
			catch (Exception ex)
			{
				return new AccountResponse
				{
					Success = false,
					Error = ex.Message
				};
			}
		}

		#endregion

		public async Task LogoutAsync()
		{
			NSError error;
			if (Firebase.Auth.Auth.DefaultInstance.SignOut(out error))
			{
				if (Settings.Current.AuthType == "facebook")
				{
					new Facebook.LoginKit.LoginManager().LogOut();
				}
				else if (Settings.Current.AuthType == "google")
				{
					Google.SignIn.SignIn.SharedInstance.SignOutUser();
				}
				else if (Settings.Current.AuthType == "github")
				{
					await LogoutGithub();
				}
				else if (Settings.Current.AuthType == "twitter")
				{
					await LogoutTwitter();
				}

				Settings.Current.AuthType = string.Empty;
			}
			else
			{
				throw new Exception(error.LocalizedDescription);
			}
		}

		#region Google Sign In
		TaskCompletionSource<GoogleUser> googleSignInTask;

		public async Task<AccountResponse> LoginWithGoogle()
		{
			googleSignInTask = new TaskCompletionSource<GoogleUser>();

			Google.SignIn.SignIn.SharedInstance.Delegate = this;
			Google.SignIn.SignIn.SharedInstance.UIDelegate = this;

			Google.SignIn.SignIn.SharedInstance.SignInUser();
			var googleUser = await googleSignInTask.Task;

			var googleAuth = Firebase.Auth.GoogleAuthProvider.GetCredential(
				googleUser.Authentication.IdToken,
				googleUser.Authentication.AccessToken
			);

			Google.SignIn.SignIn.SharedInstance.Delegate = null;
			Google.SignIn.SignIn.SharedInstance.UIDelegate = null;

			var firebaseResult = await LoginToFirebase(googleAuth);
			//googleUser.Authentication.tok

			if (firebaseResult.Success)
			{
				Settings.Current.AuthType = "google";
				//firebaseResult.User.Email = emailAddress;
			}

			return firebaseResult;
		}

		[Export("signIn:didSignInForUser:withError:")]
		public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
		{
			if (error == null)
			{
				googleSignInTask.TrySetResult(user);
			}
			else
			{
				googleSignInTask.TrySetException(new Exception(error.LocalizedDescription));
			}
		}

		[Export("signIn:presentViewController:")]
		public void ShowGoogleSignIn(SignIn signIn, UIViewController viewController)
		{
			GetViewController().PresentViewController(viewController, true, () => { });
		}

		[Export("signIn:dismissViewController:")]
		public void DismissGoogleSignIn(SignIn signIn, UIViewController viewController)
		{
			viewController.DismissViewController(true, () => { });
		}

		#endregion

		private UIViewController GetViewController()
		{
			var vc = TrackCurrentViewControllerRenderer.CurrentViewController;

			return vc;
		}

		async Task<AccountResponse> LoginToFirebase(AuthCredential credential)
		{
			try
			{
				// Authenticate with Firebase using the credential
				var user = (await Auth.DefaultInstance.SignInAndRetrieveDataWithCredentialAsync(credential))?.User;

				// Do your magic to handle authentication result
				var split = user.DisplayName.Split(' ');

				var account = new AccountResponse
				{
					Success = true,
					User = new Clients.Portable.User()
					{
						Id = user.Uid,
						Email = user.Email,
						FirstName = split?.FirstOrDefault(),
						LastName = split?.LastOrDefault(),
						AvatarUrl = user.PhotoUrl.ToString()
					}
				};

				// extract email and/or photo url if null
				foreach (var provider in user.ProviderData)
				{
					if (string.IsNullOrEmpty(account.User.Email))
					{
						account.User.Email = provider.Email;
					}

					if (string.IsNullOrEmpty(account.User.AvatarUrl))
					{
						account.User.AvatarUrl = provider.PhotoUrl.ToString();
					}
				}

				return account;
			}
			catch (Exception ex)
			{
				return new AccountResponse
				{
					Success = false,
					Error = ex.Message
				};
			}
		}
	}
}