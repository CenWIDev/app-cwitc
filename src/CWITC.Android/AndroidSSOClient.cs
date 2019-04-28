using System;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidSSOClient))]
namespace CWITC.Droid
{
	public class AndroidSSOClient : ISSOClient
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
