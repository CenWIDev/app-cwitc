using System;
using CWITC.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomFontNavigationPageRenderer))]
namespace CWITC.iOS
{
    // https://blog.xamarin.com/custom-fonts-in-ios/
    public class CustomFontNavigationPageRenderer : NavigationRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (this.NavigationBar == null) return;

            SetNavBarItems();
        }

        private void SetNavBarItems()
        {
            var navPage = this.Element as NavigationPage;

            if (navPage == null) return;

            var tint = UIColor.FromRGB(236, 47, 75);
            var textAttributes = new UITextAttributes()
            {
                TextColor = tint,
                Font = UIFont.SystemFontOfSize(14.0f, UIFontWeight.Regular)
            };

            var textAttributesHighlighted = new UITextAttributes()
            {
                TextColor = tint,
                Font = UIFont.SystemFontOfSize(14.0f, UIFontWeight.Regular)
            };

            UIBarButtonItem.Appearance.SetTitleTextAttributes(textAttributes,
                UIControlState.Normal);
            UIBarButtonItem.Appearance.SetTitleTextAttributes(textAttributesHighlighted,
                UIControlState.Highlighted);

            //UIBarButtonItem.Appearance.seti
        }

    }
}
