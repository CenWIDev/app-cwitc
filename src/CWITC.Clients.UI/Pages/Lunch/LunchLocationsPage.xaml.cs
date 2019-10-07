using System;
using System.Collections.Generic;
using System.Web;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CWITC.Clients.UI
{
	public partial class LunchLocationsPage : ContentPage
	{
		LunchLocationsViewModel vm;
		LunchLocationsViewModel ViewModel => vm ?? (vm = BindingContext as LunchLocationsViewModel);

		public LunchLocationsPage()
		{
			InitializeComponent();

			BindingContext = new LunchLocationsViewModel(this.Navigation);

			ListViewLocations.ItemTapped += (sender, e) => ListViewLocations.SelectedItem = null;
			ListViewLocations.ItemSelected += (sender, e) =>
			{
				var ev = ListViewLocations.SelectedItem as LunchLocation;
				if (ev == null)
					return;

				ShowDetail(ev);

				ListViewLocations.SelectedItem = null;
			};
		}

		async void ShowDetail(LunchLocation ev)
		{
			var result = await this.DisplayActionSheet(ev.Name, "Cancel", null,
				"View Website",
				"Open Maps");

			//var addr = ev.Address.Replace("\n", " ").Replace(" ", "+").Replace(",", string.Empty);
			var q = HttpUtility.UrlEncode($"{ev.Name} Stevens Point, WI");

			if (result == "Cancel")
				return;

			Uri uri;
			if (result == "View Website")
			{
				uri = new Uri(ev.Website);
			}
			else if (Xamarin.Forms.Device.RuntimePlatform == Device.iOS)
			{
				// launch address w/ walking directions
				uri = new Uri($"http://maps.apple.com/?q={q}");
			}
			else if (Xamarin.Forms.Device.RuntimePlatform == Device.Android)
			{
				uri = new Uri($"geo:0,0?q={q}");
			}
			else return;

			Xamarin.Forms.Device.OpenUri(uri);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (ViewModel.Locations.Count == 0)
				ViewModel.LoadLocationsCommand.Execute(false);
		}
	}
}