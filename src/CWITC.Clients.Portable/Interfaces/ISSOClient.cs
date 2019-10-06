using System;
using System.Threading.Tasks;

namespace CWITC.Clients.Portable
{
    public interface ISSOClient
    {
        Task<AccountResponse> LoginWithFacebook();

        Task<AccountResponse> LoginWithGoogle();

		Task<AccountResponse> LoginWithTwitter();

		Task<AccountResponse> LoginWithGithub();

		Task LogoutAsync();
    }
}