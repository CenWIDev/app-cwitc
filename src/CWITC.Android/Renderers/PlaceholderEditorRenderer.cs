using CWITC.Clients.UI;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PlaceholderEditor), typeof(CWITC.Droid.PlaceholderEditorRenderer))]
namespace CWITC.Droid
{
	// https://gist.github.com/mmierzwa/252bfaae4c8db12a358a42473283c002#file-placehodereditorrenderer-cs

	public class PlaceholderEditorRenderer : EditorRenderer
	{
		public PlaceholderEditorRenderer(Android.Content.Context context) : base(context) { }

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (Element == null)
				return;

			var element = (PlaceholderEditor)Element;

			Control.Hint = element.Placeholder;
			Control.SetHintTextColor(element.PlaceholderColor.ToAndroid());
		}
	}
}