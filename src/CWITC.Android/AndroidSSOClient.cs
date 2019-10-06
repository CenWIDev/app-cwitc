using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using CWITC.Clients.Portable;
using CWITC.Droid;
using Firebase.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidSSOClient))]
namespace CWITC.Droid
{
	public partial class AndroidSSOClient : Java.Lang.Object,
		ISSOClient,
		GoogleApiClient.IConnectionCallbacks,
		GoogleApiClient.IOnConnectionFailedListener
	{
		private TaskCompletionSource<GoogleSignInAccount> googleSignInTask;
		private GoogleApiClient _apiClient;

		public async Task LogoutAsync()
		{
			bool success = true;
			string error = null;
			try
			{
				FirebaseAuth.Instance.SignOut();
					}
			catch(Exception ex)
			{
				error = ex.Message;
				success = false;
			}

			if (success)
			{
				if (Settings.Current.AuthType == "facebook")
				{
					await this.LogoutFacebook();
				}
				else if (Settings.Current.AuthType == "google")
				{
					//GoogleSignInApi.
					//Google.SignIn.SignIn.SharedInstance.SignOutUser();
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
				throw new Exception(error);
			}
		}

		#region Google Login

		public async Task<AccountResponse> LoginWithGoogle()
		{
			try
			{
				googleSignInTask = new TaskCompletionSource<GoogleSignInAccount>();

				var activity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;
				var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
					.RequestIdToken(activity.GetString(Resource.String.default_web_client_id))
					.RequestEmail()
					.Build();

				_apiClient = new GoogleApiClient.Builder(activity)
					.AddConnectionCallbacks(this)
					.AddOnConnectionFailedListener(this)
					.AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API, gso)
					.Build();

				_apiClient.Connect();

				var googleAccount = await googleSignInTask.Task;

				var credential = Firebase.Auth.GoogleAuthProvider.GetCredential(
					googleAccount.IdToken,
					null);

				var firebaseResult = await LoginToFirebase(credential);

				Settings.Current.AuthType = "google";

				return firebaseResult;
			}
			catch (System.Exception ex)
			{
				return new AccountResponse
				{
					Success = false,
					Error = ex.Message
				};
			}
		}

		void GoogleApiClient.IConnectionCallbacks.OnConnected(Bundle connectionHint)
		{
			var activity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity as MainActivity;
			activity.GoogleSignIn(_apiClient, googleSignInTask);
		}

		void GoogleApiClient.IConnectionCallbacks.OnConnectionSuspended(int cause)
		{
		}

		void GoogleApiClient.IOnConnectionFailedListener.OnConnectionFailed(ConnectionResult result)
		{
		}

		#endregion

		async Task<AccountResponse> LoginToFirebase(AuthCredential credential)
		{
			try
			{
				var signinResult = await FirebaseAuth.Instance.SignInWithCredentialAsync(credential);
				var user = signinResult.User;

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
			catch (System.Exception ex)
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
