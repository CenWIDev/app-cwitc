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
            TaskCompletionSource<string> tokenTask = new TaskCompletionSource<string>();
            var topVC = GetViewController();

            new Facebook.LoginKit.LoginManager().LogInWithReadPermissions(
                new string[] { "public_profile" },
                topVC,
                (Facebook.LoginKit.LoginManagerLoginResult result, NSError error) =>
                {
					if (error != null)
					{
                    
						//NSLog(@"Process error");
					}
					else if (result.IsCancelled)
					{
                        tokenTask.SetCanceled();
						//NSLog(@"Cancelled");

					}
					else
					{
                        var token = Facebook.CoreKit.AccessToken.CurrentAccessToken;
                        tokenTask.SetResult(accessToken.TokenString);
                        //accessToken.pr
                    //result.acc
						//NSLog(@"Logged in");
					}
                });

            string accessToken = await tokenTask.Task;

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
