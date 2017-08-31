using System;
using System.Threading.Tasks;

namespace CWITC.Clients.Portable
{
    public interface ISSOClient
    {
        Task<AccountResponse> LoginAnonymously();

        Task<AccountResponse> LoginWithFacebook();

        Task<AccountResponse> LoginWithGoogle();

        Task LogoutAsync();
    }
}

