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

            this.Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Position(44.523411, -89.583897),
                Distance.FromMiles(0.25)));
            
            ToolbarItem showList = null;
            ToolbarItem showMap = null;

            showList = new ToolbarItem("Show List", "ic_view_list.png", () => 
            {
                ListViewLocations.IsVisible = true;
                MapWrapper.IsVisible = false; 

                ToolbarItems.Remove(showList);
                ToolbarItems.Add(showMap);
            });

			showMap = new ToolbarItem("Show Map", "ic_map.png", () =>
			{
                ListViewLocations.IsVisible = false;
				MapWrapper.IsVisible = true;

                ToolbarItems.Remove(showMap);
                ToolbarItems.Add(showList);
			});

            ToolbarItems.Add(showMap);

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

            MapBehavior.ShowDetailCommand = new Command<ILocationViewModel>((x) =>
            {
                var ev = (x as LunchLocationsViewModel.LunchLocationPinViewModel);

                ShowDetail(ev.Location);
            });
        }

        async void ShowDetail(LunchLocation ev)
        {
			var eventDetails = new LunchLocationDetailsPage();

			eventDetails.Location = ev;
			App.Logger.TrackPage(AppPage.LunchLocation.ToString(), ev.Name);
			await NavigationService.PushAsync(Navigation, eventDetails);
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();

                if (ViewModel.Locations.Count == 0)
                ViewModel.LoadLocationsCommand.Execute(false);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}
    }
}
