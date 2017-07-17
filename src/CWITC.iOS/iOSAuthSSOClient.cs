using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Auth0.OidcClient;
using CoreGraphics;
using CWITC.Clients.Portable;
//using Firebase.Auth;
using FormsToolkit;
using Foundation;
using SafariServices;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;

namespace CWITC.iOS
{
    public partial class iOSAuthSSOClient : NSObject, ISFSafariViewControllerDelegate
    {
        public Task<AccountResponse> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountResponse> LoginWithFacebook()
        {
            TaskCompletionSource<AccountResponse> tcs = new TaskCompletionSource<AccountResponse>();

            var auth = new OAuth2Authenticator(
                clientId: "8c9063efe1ff709ecc09",
                scope: "user:email",
                authorizeUrl: new Uri("http://github.com/login/oauth/authorize"),
                redirectUrl: new Uri("org.cenwidev.cwitc://localhost"));

            var url = await auth.GetInitialUrlAsync();
            //auth.plat
            var loginViewController = auth.GetUI();

            auth.Completed += (sender, eventArgs) =>
            {
                // We presented the UI, so it's up to us to dimiss it on iOS.
                loginViewController.DismissViewController(true, null);

                if (eventArgs.IsAuthenticated)
                {
                    tcs.SetResult(new AccountResponse
                    {
                        Success = true,
                        User = new User
                        {

                        },
                        Token = eventArgs.Account.Properties["access_token"]
                    });
                    // Use eventArgs.Account to do wonderful things
                }
                else
                {
                    tcs.SetResult(new AccountResponse
                    {
                        Success = false
                    });
                }
            };

            var vc = new Xamarin.Auth.XamarinForms.AuthenticatorPage()
            {
                Authenticator = auth,
            }.CreateViewController();

            GetViewController().PresentViewController(vc, true, () => { });
			return await tcs.Task;
		}
       

        public Task LogoutAsync()
        {
            NSError error;
            //Firebase.Auth.Auth.DefaultInstance.SignOut(out error);

            // todo: handle errors

            return Task.CompletedTask;
        }

        private UIViewController GetViewController()
        {
            var vc = TrackCurrentViewControllerRenderer.CurrentViewController;

            return vc;
        }
    }
}
