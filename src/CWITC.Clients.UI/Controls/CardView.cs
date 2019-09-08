using Xamarin.Forms;

namespace CWITC.Clients.UI
{
    public class CardView : Frame
    {
        public CardView()
        {
            Padding = 0;
            if (Device.RuntimePlatform == "iOS")
            {
                HasShadow = false;
				BorderColor = Color.Transparent;
                BackgroundColor = Color.Transparent;
            }
        }
    }
}

