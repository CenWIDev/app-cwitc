using System;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(iOSSSOClient))]
namespace CWITC.iOS
{
	public class iOSSSOClient : ISSOClient
	{
		public Task<AccountResponse> LoginAnonymously()
		{
			throw new NotImplementedException();
		}

		public Task<AccountResponse> LoginWithFacebook()
		{
			throw new NotImplementedException();
		}

		public Task<AccountResponse> LoginWithGoogle()
		{
			throw new NotImplementedException();
		}

		public Task LogoutAsync()
		{
			throw new NotImplementedException();
		}
	}
}
