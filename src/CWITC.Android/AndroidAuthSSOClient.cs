using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using Auth0.OidcClient;
using CWITC.Clients.Portable;
using FormsToolkit;
using IdentityModel.OidcClient;
using Plugin.CurrentActivity;

namespace CWITC.Droid
{
	public partial class AndroidAuthSSOClient : ISSOClient
	{
        public async Task<AccountResponse> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

		public async Task<AccountResponse> LoginWithFacebook()
		{
			var client = new Auth0Client(new Auth0ClientOptions
			{
				Domain = "cwitc.auth0.com",
				ClientId = "r2xGTXLZeEgCmqYIgLOaRJwD1sDoySzh",
                Activity = CrossCurrentActivity.Current.Activity
			});

            AuthorizeState authorizeState = await client.PrepareLoginAsync();

			var uri = Android.Net.Uri.Parse(authorizeState.StartUrl);
			var intent = new Intent(Intent.ActionView, uri);
			intent.AddFlags(ActivityFlags.NoHistory);
            CrossCurrentActivity.Current.Activity.StartActivity(intent);

            TaskCompletionSource<AccountResponse> tcs = new TaskCompletionSource<AccountResponse>();

            MessagingService.Current.Subscribe(MessageKeys.LoginCallback, async (IMessagingService arg1, string dataString) =>
            {
                var loginResult = await client.ProcessResponseAsync(dataString, authorizeState);

                if (loginResult.IsError)
                {
                    Debug.WriteLine($"An error occurred during login: {loginResult.Error}");

                    tcs.SetResult(new AccountResponse { Error = loginResult.Error, Success = false });
                }
                else
                {
                    string token = loginResult.IdentityToken;
                    string accesstoken = loginResult.AccessToken;
                    string name = loginResult.User.FindFirst(c => c.Type == "name")?.Value;
                    string email = loginResult.User.FindFirst(c => c.Type == "email")?.Value;

                    tcs.SetResult(new AccountResponse
                    {
                        Success = true,
                        Token = token,
                        User = new User
                        {
                            FirstName = name.Split(' ')[0],
                            LastName = name.Split(' ')[1],
                            Email = email
                        }
                    });
                }
            });

            return await tcs.Task;
		}

		public Task LogoutAsync()
		{
			var redirect = "org.cenwidev.cwitc%3A%2F%2Fcwitc.auth0.com/android/org.cenwidev.cwitc/logout";
			string logoutUri = $"https://cwitc.auth0.com/v2/logout?returnTo={redirect}";

			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

			MessagingService.Current.Subscribe(MessageKeys.LogoutCallback, (IMessagingService arg1) =>
			{

				tcs.SetResult(true);

			});

            var uri = Android.Net.Uri.Parse(logoutUri);
			var intent = new Intent(Intent.ActionView, uri);
			intent.AddFlags(ActivityFlags.NoHistory);
			CrossCurrentActivity.Current.Activity.StartActivity(intent);

			return tcs.Task;
		}
	}
}
