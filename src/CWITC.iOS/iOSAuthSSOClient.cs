using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Auth0.OidcClient;
using CoreGraphics;
using CWITC.Clients.Portable;
using Firebase.Auth;
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
            TaskCompletionSource<string> tokenTask = new TaskCompletionSource<string>();
            var topVC = GetViewController();

            new Facebook.LoginKit.LoginManager().LogInWithReadPermissions(
                new string[] { "public_profile email" },
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
                        tokenTask.SetResult(token.TokenString);
                    }
                });

            string accessToken = await tokenTask.Task;

            TaskCompletionSource<string> getEmailTask = new TaskCompletionSource<string>();
            // gets the email & name for this user
            var graphRequest = new Facebook.CoreKit.GraphRequest("me", NSDictionary.FromObjectAndKey(new NSString("id,email"), new NSString("fields")));
            graphRequest.Start((connection, result, error) =>
                {
                    if (error != null)
                    {
                        //result.
                    }
                    var email = (NSString)result.ValueForKey(new NSString("email"));
                    var name = (NSString)result.ValueForKey(new NSString("name"));

                    getEmailTask.SetResult(email);
                });

            string emailAddress = await getEmailTask.Task;

            // Get access token for the signed-in user and exchange it for a Firebase credential
            var credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
            var firebaseResult = await LoginToFirebase(credential);
            if (firebaseResult.Success)
            {
                firebaseResult.User.Email = emailAddress;
            }

            return firebaseResult;
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

        async Task<AccountResponse> LoginToFirebase(AuthCredential credential)
        {
            TaskCompletionSource<AccountResponse> tcs = new TaskCompletionSource<AccountResponse>();

            // Authenticate with Firebase using the credential
            Auth.DefaultInstance.SignIn(credential, (user, error) =>
            {
                if (error != null)
                {
                    AuthErrorCode errorCode;
                    if (IntPtr.Size == 8) // 64 bits devices
                        errorCode = (AuthErrorCode)((long)error.Code);
                    else // 32 bits devices
                        errorCode = (AuthErrorCode)((int)error.Code);

                    // Posible error codes that SignIn method with credentials could throw
                    // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                    switch (errorCode)
                    {
                        case AuthErrorCode.InvalidCredential:
                        case AuthErrorCode.InvalidEmail:
                        case AuthErrorCode.OperationNotAllowed:
                        case AuthErrorCode.EmailAlreadyInUse:
                        case AuthErrorCode.UserDisabled:
                        case AuthErrorCode.WrongPassword:
                        default:
                            // Print error
                            break;
                    }

                    tcs.SetResult(new AccountResponse
                    {
                        Success = false,
                        Error = error.LocalizedDescription
                    });
                }
                else
                {
                    // Do your magic to handle authentication result
                    var split = user.DisplayName.Split(' ');

                    tcs.SetResult(new AccountResponse
                    {
                        Success = true,
                        User = new Clients.Portable.User()
                        {
                            Id = user.Uid,
                            Email = user.Email,
                            FirstName = split?.FirstOrDefault(),
                            LastName = split?.LastOrDefault()
                        }
                    });
                }
            });

            return await tcs.Task;
        }
    }
}