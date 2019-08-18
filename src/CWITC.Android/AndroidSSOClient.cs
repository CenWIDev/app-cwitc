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

		public Task<AccountResponse> LoginWithFacebook()
		{
			throw new NotImplementedException();
			//var mainActivity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity as MainActivity;

			//var tokenTask = new TaskCompletionSource<AccessToken>();

			//var loginManager = DeviceLoginManager.Instance;

			//loginManager.RegisterCallback(
			//	mainActivity.CallbackManager, new FacebookLoginCallback(tokenTask));

			//loginManager
			//	   .LogInWithReadPermissions(
			//		   Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity,
			//		 new List<string>
			//		{
			//			"public_profile",
			//			"email"
			//		});
			//try
			//{
			//	var accessToken = await tokenTask.Task;
			//	loginManager.UnregisterCallback(mainActivity.CallbackManager);

			//	TaskCompletionSource<string> getEmailTask = new TaskCompletionSource<string>();

			//	Bundle parameters = new Bundle();
			//	parameters.PutString("fields", "id,email");
			//	var graphRequestResult = (await new GraphRequest(accessToken, "me", parameters, HttpMethod.Get)
			//		.ExecuteAsync()
			//		.GetAsync() as ArrayList).ToArray();

			//	var graphResponse = graphRequestResult.FirstOrDefault() as GraphResponse;

			//	string emailAddress = graphResponse.JSONObject.GetString("email");

			//	var credential = FacebookAuthProvider.GetCredential(accessToken.Token);

			//	var firebaseResult = await LoginToFirebase(credential);
			//	if (firebaseResult.Success)
			//	{
			//		Settings.Current.AuthType = "facebook";
			//		firebaseResult.User.Email = emailAddress;
			//	}

			//	return firebaseResult;
			//}
			//catch (System.Exception ex)
			//{
			//	return new AccountResponse
			//	{
			//		Success = false,
			//		Error = ex.Message
			//	};
			//}
		}

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
					//.EnableAutoManage() //activity,  this /* OnCon    nectionFailedListener */)
					.AddConnectionCallbacks(this)
					.AddOnConnectionFailedListener(this)
					.AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API, gso)
					.Build();

				//_apiClient = new GoogleApiClient.Builder(Xamarin.Forms.Forms.Context)

				//   .AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API)
				//    .Build();

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

		public Task LogoutAsync()
		{
			throw new NotImplementedException();
		}

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

		#region Google Login

		void GoogleApiClient.IConnectionCallbacks.OnConnected(Bundle connectionHint)
		{
			var activity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity as MainActivity;
			activity.GoogleSignIn(_apiClient, googleSignInTask);

			//Xamarin.Forms.Forms.Context.StartActivityForResult(signInIntent, RC_SIGN_IN);
			//Intent signInIntent =  GoogleSignInApi. .getSignInIntent(mGoogleApiClient);
			//startActivityForResult(signInIntent, RC_AUTHORIZE_CONTACTS);
		}

		void GoogleApiClient.IConnectionCallbacks.OnConnectionSuspended(int cause)
		{
			//throw new NotImplementedException();
		}

		void GoogleApiClient.IOnConnectionFailedListener.OnConnectionFailed(ConnectionResult result)
		{
			//throw new NotImplementedException();
		}

		#endregion
	}
}
