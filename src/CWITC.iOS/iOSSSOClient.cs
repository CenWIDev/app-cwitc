using System;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.iOS;
using Firebase.Auth;
using Foundation;
using Google.SignIn;
using SafariServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(iOSAuthSSOClient))]
namespace CWITC.iOS
{
	public class iOSAuthSSOClient : NSObject, ISSOClient,
		ISFSafariViewControllerDelegate, ISignInDelegate, ISignInUIDelegate
	{
		static iOSAuthSSOClient()
		{
			var googleServiceDictionary = NSDictionary.FromFile("GoogleService-Info.plist");
			Google.SignIn.SignIn.SharedInstance.ClientID = googleServiceDictionary["CLIENT_ID"].ToString();
		}

		public async Task<AccountResponse> LoginWithFacebook()
		{
			try
			{
				var topVC = GetViewController();

				var fbLogin = new Facebook.LoginKit.LoginManager();

				var loginResult = fbLogin.LogInWithReadPermissionsAsync(new string[] { "public_profile email" }, topVC);

				while(loginResult.Status != TaskStatus.RanToCompletion &&
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
			catch(Exception ex)
			{
				return new AccountResponse
				{
					Success = false,
					Error = ex.Message
				};
			}
		}

		public Task<AccountResponse> LoginWithGithub()
		{
			throw new NotImplementedException();
		}

		public Task<AccountResponse> LoginWithTwitter()
		{
			throw new NotImplementedException();
		}

		public Task LogoutAsync()
		{
			NSError error;
			if (Firebase.Auth.Auth.DefaultInstance.SignOut(out error))
			{
				if (Settings.Current.AuthType == "facebook")
				{
					new Facebook.LoginKit.LoginManager().LogOut();
					Settings.Current.AuthType = string.Empty;
				}
				else if(Settings.Current.AuthType == "google")
				{
					Google.SignIn.SignIn.SharedInstance.SignOutUser();
					Settings.Current.AuthType = string.Empty;
				}
			}
			else
			{
				throw new Exception(error.LocalizedDescription);
			}

			return Task.CompletedTask;
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

				return new AccountResponse
				{
					Success = true,
					User = new Clients.Portable.User()
					{
						Id = user.Uid,
						Email = user.Email,
						FirstName = split?.FirstOrDefault(),
						LastName = split?.LastOrDefault()
					}
				};
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
