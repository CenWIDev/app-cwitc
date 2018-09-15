using System;
using System.Threading.Tasks;

namespace CWITC.Clients.Portable
{
    public interface IAuthClient
    {
		Task<AccountResponse> LoginAsync();

        Task LogoutAsync();
    }
}