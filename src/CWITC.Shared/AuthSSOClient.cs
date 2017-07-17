using System;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using Xamarin.Auth;
#if __ANDROID__
namespace CWITC.Droid
{
    public partial class AndroidAuthSSOClient : ISSOClient
#elif __IOS__
namespace CWITC.iOS
{
    public partial class iOSAuthSSOClient : ISSOClient
#endif

    {
        
    }
}
