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

		public async Task LogoutAsync()
		{
			NSError error;
			if (Firebase.Auth.Auth.DefaultInstance.SignOut(out error))
			{
				if (Settings.Current.AuthType == "facebook")
				{
					await LogoutFacebook();
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

		UIViewController GetViewController()
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