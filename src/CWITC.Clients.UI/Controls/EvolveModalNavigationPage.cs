
using Xamarin.Forms;

namespace CWITC.Clients.UI
{
    public class EvolveModalNavigationPage : EvolveNavigationPage
    {
        public EvolveModalNavigationPage(Page root) : base(root)
        {
            Init();
            Title = root.Title;
            Icon = root.Icon;

            var toolbarDone = new ToolbarItem()
            {
                Text = "Done",
				Command = new Command(async () =>
				{
					await Navigation.PopModalAsync();
                })
            };

			if (Device.OS != TargetPlatform.iOS)
				toolbarDone.Icon = "toolbar_close.png";

            ToolbarItems.Add(toolbarDone);
        }

        public EvolveModalNavigationPage()
        {
            Init();
        }

        void Init()
        {
            
    //        if (Device.OS == TargetPlatform.iOS)
    //        {
				//BarBackgroundColor = Color.FromHex("FAFAFA");
            //}
            //else
            //{   
                BarBackgroundColor = (Color)Application.Current.Resources["Primary"];
                BarTextColor = (Color)Application.Current.Resources["NavigationText"];
            //}
        }
    }
}

