using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using Auth0.OidcClient;
using CWITC.Clients.Portable;
using Firebase.Auth;
using FormsToolkit;
using IdentityModel.OidcClient;
using Plugin.CurrentActivity;

namespace CWITC.Droid
{
    public partial class AndroidAuthSSOClient : ISSOClient
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
            catch (Exception ex)
            {
                return new AccountResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
            //authResult.
        }

        public async Task<AccountResponse> LoginWithFacebook()
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
            }
            catch(Exception ex)
            {
                // todo: handle errors
            }

            return Task.CompletedTask;
        }
    }
}
