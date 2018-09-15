using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Auth0.OidcClient;
using CWITC.Clients.Portable;
using Firebase.Auth;
using FormsToolkit;
using IdentityModel.OidcClient;
using Java.Lang;
using Java.Util;
//using Plugin.CurrentActivity;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace CWITC.Droid
{
    public partial class AndroidAuthSSOClient :
        Java.Lang.Object,
        IAuthClient,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener
    {
        TaskCompletionSource<GoogleSignInAccount> googleSignInTask;
        private GoogleApiClient _apiClient;

        public async Task<AccountResponse> LoginAnonymously()
        {
            try
            {
                var authResult = await FirebaseAuth.Instance.SignInAnonymouslyAsync();

                var user = authResult.User;

                Settings.Current.AuthType = "anonymous";

                return new AccountResponse
                {
                    User = new Clients.Portable.User
                    {
                        IsAnonymous = true,
                        Id = user.Uid
                    },
                    Success = true
                };
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

        public async Task<AccountResponse> LoginWithGoogle()
        {
            try
            {
                googleSignInTask = new TaskCompletionSource<GoogleSignInAccount>();

                GetGoogleApiClient();

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

        private void GetGoogleApiClient()
        {
            if (_apiClient == null)
            {
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

                _apiClient.Connect();
            }
        }

        public async Task<AccountResponse> LoginWithFacebook()
        {
            var mainActivity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity as MainActivity;

            var tokenTask = new TaskCompletionSource<AccessToken>();

            var loginManager = DeviceLoginManager.Instance;

            loginManager.RegisterCallback(
                mainActivity.CallbackManager, new FacebookLoginCallback(tokenTask));

            loginManager
                   .LogInWithReadPermissions(
                       Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity,
                     new List<string>
                    {
                        "public_profile",
                        "email"
                    });
            try
            {
                var accessToken = await tokenTask.Task;
                loginManager.UnregisterCallback(mainActivity.CallbackManager);

                TaskCompletionSource<string> getEmailTask = new TaskCompletionSource<string>();

                Bundle parameters = new Bundle();
                parameters.PutString("fields", "id,email");
                var graphRequestResult = (await new GraphRequest(accessToken, "me", parameters, HttpMethod.Get)
                    .ExecuteAsync()
                    .GetAsync() as ArrayList).ToArray();

                var graphResponse = graphRequestResult.FirstOrDefault() as GraphResponse;

                string emailAddress = graphResponse.JSONObject.GetString("email");

                var credential = FacebookAuthProvider.GetCredential(accessToken.Token);

                var firebaseResult = await LoginToFirebase(credential);
                if (firebaseResult.Success)
                {
                    Settings.Current.AuthType = "facebook";
                    firebaseResult.User.Email = emailAddress;
                }

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

        public async Task LogoutAsync()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();

                if (Settings.Current.AuthType == "facebook")
                {
                    var loginManager = DeviceLoginManager.Instance;
                    loginManager.LogOut();
                }

				if (Settings.Current.AuthType == "google")
				{
                    GetGoogleApiClient();

                    TaskCompletionSource<Statuses> signoutTaks = new TaskCompletionSource<Statuses>();
                        
                    Auth.GoogleSignInApi.SignOut(_apiClient)
                        .SetResultCallback<IResult>(result => 
                        {
                            signoutTaks.SetResult(result.Status);
                        });

                    var logoutStatus = await signoutTaks.Task;
                    _apiClient = null;
				}

                Settings.Current.AuthType = string.Empty;
            }
            catch (System.Exception ex)
            {
                // todo: handle errors
            }
        }

        async Task<AccountResponse> LoginToFirebase(AuthCredential credential)
        {
            try
            {
                var signinResult = await FirebaseAuth.Instance.SignInWithCredentialAsync(credential);
                var user = signinResult.User;

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

        #region Facebook Login

        class FacebookLoginCallback : Java.Lang.Object, IFacebookCallback
        {
            TaskCompletionSource<AccessToken> tokenTask;
            public FacebookLoginCallback(TaskCompletionSource<AccessToken> tokenTask)
            {
                this.tokenTask = tokenTask;
            }

            void IFacebookCallback.OnCancel()
            {
                tokenTask.TrySetCanceled();
            }

            void IFacebookCallback.OnError(FacebookException error)
            {
                tokenTask.TrySetException(error);
            }

            void IFacebookCallback.OnSuccess(Java.Lang.Object result)
            {
                var loginResult = result as Xamarin.Facebook.Login.LoginResult;

                var accessToken = loginResult.AccessToken;

                tokenTask.TrySetResult(accessToken);
                //throw new NotImplementedException();
            }
        }

        #endregion

    }
}
