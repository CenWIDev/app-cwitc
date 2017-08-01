﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Runtime;
using Auth0.OidcClient;
using CWITC.Clients.Portable;
using Firebase.Auth;
using FormsToolkit;
using IdentityModel.OidcClient;
using Java.Lang;
using Plugin.CurrentActivity;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace CWITC.Droid
{
    public partial class AndroidAuthSSOClient : Java.Lang.Object, ISSOClient
    {
        public async Task<AccountResponse> LoginAnonymously()
        {
            try
            {
                var authResult = await FirebaseAuth.Instance.SignInAnonymouslyAsync();

                var user = authResult.User;

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

        public async Task<AccountResponse> LoginWithFacebook()
        {
            var tokenTask = new TaskCompletionSource<AccessToken>();

            var callbackManager = CallbackManagerFactory.Create();

            var loginManager = DeviceLoginManager.Instance;
            loginManager.RegisterCallback(
                callbackManager, new FacebookLoginCallback(tokenTask));

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
                loginManager.UnregisterCallback(callbackManager);

                TaskCompletionSource<string> getEmailTask = new TaskCompletionSource<string>();

                var graphResponse = (await new GraphRequest(accessToken, "me")
                    .ExecuteAsync()
                    .GetAsync()) as GraphResponse;
                string emailAddress = graphResponse.JSONObject.Get("email").ToString();
                //string name = graphResponse.JSONObject.Get("name").ToString();

                var credential = FacebookAuthProvider.GetCredential(accessToken.Token);

                var firebaseResult = await LoginToFirebase(credential);
                if (firebaseResult.Success)
                {
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

        public Task LogoutAsync()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();

                var loginManager = DeviceLoginManager.Instance;
                loginManager.LogOut();
            }
            catch (System.Exception ex)
            {
                // todo: handle errors
            }

            return Task.CompletedTask;
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

    }
}
