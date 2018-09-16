using System;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataStore.Abstractions;
using FormsToolkit;
using IdentityModel.OidcClient;
using Xamarin.Forms;
#if __ANDROID__
namespace CWITC.Droid
#elif __IOS__
namespace CWITC.iOS
#endif
{
	public partial class Auth0Client : IAuthClient
	{
		public async Task LoginAsync()
		{
			var loginResult = await CreateClient().LoginAsync();

			await ProcessResult(loginResult);
		}

		public async Task LogoutAsync()
		{
			await CreateClient().LogoutAsync();
		}

		private Auth0.OidcClient.Auth0Client CreateClient() =>
			new Auth0.OidcClient.Auth0Client(new Auth0.OidcClient.Auth0ClientOptions
			{
				Domain = "cwitc.auth0.com",
				ClientId = "r2xGTXLZeEgCmqYIgLOaRJwD1sDoySzh",
				Scope = "openid profile email"
			});

		private async Task ProcessResult(LoginResult result)
		{
			var logger = DependencyService.Get<ILogger>();
			if (!result.IsError)
			{
				var storeManager = DependencyService.Get<IStoreManager>();

				var id = result.User.FindFirst(c => c.Type == "sub")?.Value;
				var firstName = result.User.FindFirst(c => c.Type == "given_name")?.Value;
				var lastName = result.User.FindFirst(c => c.Type == "family_name")?.Value;
              var email = result.User.FindFirst(c => c.Type == "email")?.Value;

				Settings.Current.UserId = id;
				Settings.Current.FirstName = firstName;
				Settings.Current.LastName = lastName;
				Settings.Current.Email = email.ToLowerInvariant();
				Settings.Current.AccessToken = result.AccessToken;
				Settings.Current.IdToken = result.IdentityToken;
				Settings.Current.AccessTokenExpiration = result.AccessTokenExpiration;

				MessagingService.Current.SendMessage(MessageKeys.LoggedIn);
				logger.Track(EvolveLoggerKeys.LoginSuccess);
				try
				{
					await storeManager.SyncAllAsync(true);
					Settings.Current.LastSync = DateTime.UtcNow;
					Settings.Current.HasSyncedData = true;
				}
				catch (Exception ex)
				{
					//if sync doesn't work don't worry it is alright we can recover later
					logger.Report(ex);
				}

				Settings.Current.FirstRun = false;
				//await Finish();
			}
			else
			{
				if (result.Error == "UserCancel")
					return;
				
				logger.Track(EvolveLoggerKeys.LoginFailure, "Reason", result.Error);
				MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
				{
					Title = "Unable to Sign in",
					Message = result.Error,
					Cancel = "OK"
				});
			}
		}
	}
}
