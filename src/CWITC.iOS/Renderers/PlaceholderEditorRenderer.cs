using System;
using Cirrious.FluentLayouts.Touch;
using CWITC.Clients.UI;
using CWITC.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PlaceholderEditor), typeof(PlaceholderEditorRenderer))]
namespace CWITC.iOS
{
	// https://gist.github.com/mmierzwa/6989929b33dbd690cce7276cad4cffa7#file-placeholdereditorrenderer-cs

	public class PlaceholderEditorRenderer : EditorRenderer
	{
		private UILabel _placeholderLabel;

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (Element == null)
				return;

			CreatePlaceholderLabel((PlaceholderEditor)Element, Control);

            Control.Ended += OnEnded;
            Control.Changed += OnChanged;
		}

		private void CreatePlaceholderLabel(PlaceholderEditor element, UITextView parent)
		{
			_placeholderLabel = new UILabel
			{
				Text = element.Placeholder,
				TextColor = element.PlaceholderColor.ToUIColor(),
				BackgroundColor = UIColor.Clear,
				//Font = UIFont.FromName(element.FontFamily, (nfloat)element.FontSize)
			};

            if (!string.IsNullOrEmpty(element.FontFamily))
                _placeholderLabel.Font = UIFont.FromName(element.FontFamily, (nfloat)element.FontSize);
            else 
                _placeholderLabel.Font = UIFont.SystemFontOfSize((nfloat)element.FontSize);

			_placeholderLabel.SizeToFit();

			parent.AddSubview(_placeholderLabel);

			parent.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
			parent.AddConstraints(
				_placeholderLabel.AtLeftOf(parent, 7),
                _placeholderLabel.AtTopOf(parent, 8)
			);
			parent.LayoutIfNeeded();

			_placeholderLabel.Hidden = parent.HasText;
		}

		private void OnEnded(object sender, EventArgs args)
		{
			if (!((UITextView)sender).HasText && _placeholderLabel != null)
				_placeholderLabel.Hidden = false;
		}

		private void OnChanged(object sender, EventArgs args)
		{
			if (_placeholderLabel != null)
				_placeholderLabel.Hidden = ((UITextView)sender).HasText;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Control.Ended -= OnEnded;
				Control.Changed -= OnChanged;

				_placeholderLabel?.Dispose();
				_placeholderLabel = null;
			}

			base.Dispose(disposing);
		}
	}
}