using System;
using Xamarin.Forms;

namespace CWITC.Clients.UI
{
	// https://gist.github.com/mmierzwa/618358f58ec200ee689b2626963d9c32

	public class PlaceholderEditor : Editor
	{
		public static BindableProperty PlaceholderProperty
			= BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(PlaceholderEditor));

		public static BindableProperty PlaceholderColorProperty
			= BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(PlaceholderEditor), Color.Gray);

		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}

		public Color PlaceholderColor
		{
			get { return (Color)GetValue(PlaceholderColorProperty); }
			set { SetValue(PlaceholderColorProperty, value); }
		}
	}
}
