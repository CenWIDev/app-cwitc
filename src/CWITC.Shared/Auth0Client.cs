using System;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
#if __ANDROID__
namespace CWITC.Droid
#elif __IOS__
namespace CWITC.iOS
#endif
{
	public partial class Auth0Client : IAuthClient
	{
		public Task<AccountResponse> LoginAsync()
		{
			return Task.FromResult(new AccountResponse());
		}

		public Task LogoutAsync()
		{
			return Task.CompletedTask;
		}
	}
}
