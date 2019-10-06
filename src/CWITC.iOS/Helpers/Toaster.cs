using Xamarin.Forms;
using CWITC.Clients.Portable;
using CWITC.iOS;
using UIKit;

[assembly: Dependency(typeof(Toaster))]
namespace CWITC.iOS
{
	public class Toaster : IToast
	{
		public void SendToast(string message)
		{
			Device.BeginInvokeOnMainThread(() =>
				{
					Xamarin.Forms.Application.Current
						.MainPage.DisplayAlert(string.Empty, message, "OK");
				});
		}
	}
}
