using System;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using Xamarin.Auth;

#if __ANDROID__
using FacebookAuthProvider = Firebase.Auth.FacebookAuthProvider;
using GitHubAuthProvider = Firebase.Auth.GithubAuthProvider;
#elif __IOS__
using FacebookAuthProvider = Firebase.Auth.FacebookAuthProvider;
using GitHubAuthProvider = Firebase.Auth.GitHubAuthProvider;
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
		private OAuth2Authenticator _facebookAuth;

		#region Facebook Auth

		public async Task<AccountResponse> LoginWithFacebook()
		{
			try
			{
				_facebookAuth = new OAuth2Authenticator(
						                   Secrets.FacebookAppId,
                        "email",
                        new Uri("https://www.facebook.com/dialog/oauth/"),
                        new Uri("https://central-wi-it-conference.firebaseapp.com/__/auth/handler"),
                        isUsingNativeUI: false,
                        getUsernameAsync: null);

        _facebookAuth.Completed += OnOAuth2AuthCompleted;
        _facebookAuth.Error += OnOAuth2AuthError;

        var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
        presenter.Login(_facebookAuth);

        this.loginTCS = new TaskCompletionSource<Account>();
        var account = await this.loginTCS.Task;

        var token = account.Properties["access_token"];

        var creds = FacebookAuthProvider.GetCredential(token);

        var firebaseResult = await this.LoginToFirebase(creds);

        if (firebaseResult.Success)
        {
            AccountStore.Create()
                .Save(account, "CWITC-Facebook-Account");

            Settings.Current.AuthType = "facebook";
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

async Task LogoutFacebook()
{
    var accountStore = AccountStore.Create();
    var accounts = await accountStore
        .FindAccountsForServiceAsync("CWITC-Facebook-Account");

    var account = accounts.FirstOrDefault();

    await accountStore.DeleteAsync(account, "CWITC-Facebook-Account");
}

#endregion

#region Github Auth

public async Task<AccountResponse> LoginWithGithub()
{
    try
    {
        _githubAuth = new OAuth2Authenticator(
            clientId: Secrets.GithubClientId,
            clientSecret: Secrets.GithubClientSecret,
            scope: "read:user user:email",
            authorizeUrl: new Uri("https://github.com/login/oauth/authorize"),
            accessTokenUrl: new Uri("https://github.com/login/oauth/access_token"),
            redirectUrl: new Uri("https://central-wi-it-conference.firebaseapp.com/__/auth/handler"),
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
            consumerKey: Secrets.TwitterClientId,
            consumerSecret: Secrets.TwitterClientSecret,
					requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
					authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
					accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
					callbackUrl: new Uri("https://central-wi-it-conference.firebaseapp.com/__/auth/handler"),
					isUsingNativeUI: false);

				_twitterAuth.Completed += OnOAuth1AuthCompleted;
				_twitterAuth.Error += OnOAuth1AuthError;
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
		
		async Task LogoutTwitter()
		{
			var accountStore = AccountStore.Create();
			var accounts = await accountStore
				.FindAccountsForServiceAsync("CWITC-Twitter-Account");

			var account = accounts.FirstOrDefault();

			await accountStore.DeleteAsync(account, "CWITC-Twitter-Account");
		}

		#endregion

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
			else
			{
				loginTCS.TrySetCanceled();
			}
		}

		void OnOAuth1AuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth1Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnOAuth1AuthCompleted;
				authenticator.Error -= OnOAuth1AuthError;
			}

			loginTCS.TrySetException(e.Exception);
		}

		void OnOAuth1AuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			var authenticator = sender as OAuth1Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnOAuth1AuthCompleted;
				authenticator.Error -= OnOAuth1AuthError;
			}

			//User user = null;
			if (e.IsAuthenticated)
			{
				loginTCS.TrySetResult(e.Account);
			}
			else
			{
				loginTCS.TrySetCanceled();
			}
		}
	}
}