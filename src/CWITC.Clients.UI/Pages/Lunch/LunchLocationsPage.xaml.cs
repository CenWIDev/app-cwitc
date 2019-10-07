using System;
using System.Collections.Generic;
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

			var addr = ev.Address.Replace("\n", " ").Replace(" ", "+");

			if (result == "Cancel")
				return;

			Uri uri = result == "View Website" ? new Uri(ev.Website) : new Uri($"geo:0,0?q={addr}");
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