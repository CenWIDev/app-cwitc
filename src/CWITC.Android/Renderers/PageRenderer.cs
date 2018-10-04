using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ContentPage), typeof(CWITC.Droid.PageRenderer))]
namespace CWITC.Droid
{
	public class PageRenderer : Xamarin.Forms.Platform.Android.PageRenderer
	{
		public PageRenderer(Android.Content.Context context) : base(context)
		{
			
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				this.Element.ForceLayout();
			}
		}
	}
}
