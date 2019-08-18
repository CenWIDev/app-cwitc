using System;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using Xamarin.Auth;

#if __ANDROID__
using GitHubAuthProvider = Firebase.Auth.GithubAuthProvider;
#endif

#if __IOS__
namespace CWITC.iOS
#elif __ANDROID__
namespace CWITC.Droid
#endif
{
#if __IOS__
	public partial class iOSAuthSSOClient
#elif __ANDROID__
	public partial class AndroidSSOClient 
#endif
		: ISSOClient
	{
		private TaskCompletionSource<Account> loginTCS;
		private OAuth2Authenticator _githubAuth;
		private OAuth1Authenticator _twitterAuth;

		#region Github Auth

		public async Task<AccountResponse> LoginWithGithub()
		{
			try
			{
				_githubAuth = new OAuth2Authenticator(
					clientId: "40c527d407bf23a53b88",
					clientSecret: "3596d1579cc2cf82c892ad9e1b17fede60d9233a",
					scope: "read:user user:email",
					authorizeUrl: new Uri("https://github.com/login/oauth/authorize"),
					redirectUrl: new Uri("https://central-wi-it-conference.firebaseapp.com/__/auth/handler"),
					accessTokenUrl: new Uri("https://github.com/login/oauth/access_token"),
					isUsingNativeUI: false,
					getUsernameAsync: null);

				_githubAuth.Completed += OnOAuth2AuthCompleted;
				_githubAuth.Error += OnOAuth2AuthError;

				var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
				presenter.Login(_githubAuth);

				this.loginTCS = new TaskCompletionSource<Account>();
				var account = await this.loginTCS.Task;

				var token = account.Properties["access_token"];
				var emailAddress = account.Properties["email"];

				var creds = GitHubAuthProvider.GetCredential(token);

				var firebaseResult = await this.LoginToFirebase(creds);

				if (firebaseResult.Success)
				{
					AccountStore.Create()
						.Save(account, "CWITC-Github-Account");

					Settings.Current.AuthType = "github";
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

		void OnOAuth2AuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnOAuth2AuthCompleted;
				authenticator.Error -= OnOAuth2AuthError;
			}

			loginTCS.TrySetException(e.Exception);
		}

		void OnOAuth2AuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnOAuth2AuthCompleted;
				authenticator.Error -= OnOAuth2AuthError;
			}

			//User user = null;
			if (e.IsAuthenticated)
			{
				loginTCS.TrySetResult(e.Account);
			}
		}

		async Task LogoutGithub()
		{
			var accountStore = AccountStore.Create();
			var accounts = await accountStore
				.FindAccountsForServiceAsync("CWITC-Github-Account");

			var account = accounts.FirstOrDefault();

			await accountStore.DeleteAsync(account, "CWITC-Github-Account");
		}

		#endregion

		#region Twitter Auth

		public async Task<AccountResponse> LoginWithTwitter()
		{
			try
			{
				_twitterAuth = new Xamarin.Auth.OAuth1Authenticator(
					"DOirvDQts9ncdphpKxH7PT8X1",
					"gUecmloB7soEsjwMNagxrKJO9nTYx7jf51FDBlKJIn63hnPyE9",
					  new Uri("https://api.twitter.com/oauth/request_token"),
					   new Uri("https://api.twitter.com/oauth/authorize"),
					   new Uri("https://api.twitter.com/oauth/access_token"),
					   new Uri("https://central-wi-it-conference.firebaseapp.com/__/auth/handler"),
					   isUsingNativeUI: false);

				_twitterAuth.Completed += OnTwitterAuthCompleted;
				_twitterAuth.Error += OnTwitterAuthError;
				var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
				presenter.Login(_twitterAuth);

				this.loginTCS = new TaskCompletionSource<Account>();
				var account = await this.loginTCS.Task;

				var token = account.Properties["oauth_token"];
				var secret = account.Properties["oauth_token_secret"];
				var creds = Firebase.Auth.TwitterAuthProvider.GetCredential(token, secret);

				var firebaseResult = await this.LoginToFirebase(creds);
				if (firebaseResult.Success)
				{
					AccountStore.Create()
						.Save(account, "CWITC-Twitter-Account");

					Settings.Current.AuthType = "twitter";
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

		private void OnTwitterAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth1Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnTwitterAuthCompleted;
				authenticator.Error -= OnTwitterAuthError;
			}

			loginTCS.TrySetException(e.Exception);
		}

		private void OnTwitterAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			var authenticator = sender as OAuth1Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnTwitterAuthCompleted;
				authenticator.Error -= OnTwitterAuthError;
			}

			//User user = null;
			if (e.IsAuthenticated)
			{
				loginTCS.TrySetResult(e.Account);
			}
		}

		async Task LogoutTwitter()
		{
			var accountStore = AccountStore.Create();
			var accounts = await accountStore
				.FindAccountsForServiceAsync("CWITC-Twitter-Account");

			var account = accounts.FirstOrDefault();

			await accountStore.DeleteAsync(account, "CWITC-Twitter-Account");
		}

		#endregion
	}
}
