using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using FormsToolkit;

namespace CWITC.Clients.UI
{
    public partial class EventDetailsPage : ContentPage
    {
        EventDetailsViewModel ViewModel => vm ?? (vm = BindingContext as EventDetailsViewModel);
        EventDetailsViewModel vm;

        public EventDetailsPage()
        {
            InitializeComponent();

            ListViewSponsors.ItemSelected += async (sender, e) => 
                {
                    var sponsor = ListViewSponsors.SelectedItem as Sponsor;
                    if(sponsor == null)
                        return;

                    var sponsorDetails = new SponsorDetailsPage
                        {
                            Sponsor = sponsor
                        };

                    App.Logger.TrackPage(AppPage.Sponsor.ToString(), sponsor.Name);
                    await NavigationService.PushAsync(Navigation, sponsorDetails);

                    ListViewSponsors.SelectedItem = null;
                };
        }

        public FeaturedEvent Event
        {
            get { return ViewModel.Event; }
            set { BindingContext = new EventDetailsViewModel(Navigation, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            vm = null;

            var adjust = Device.RuntimePlatform != "Android" ? 1 : -ViewModel.Sponsors.Count + 1;
            ListViewSponsors.HeightRequest = ListViewSponsors.RowHeight - adjust;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing(); 

            ViewModel.LoadEventDetailsCommand.Execute(null);

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async void HandleViewClicked(object sender, System.EventArgs e)
        {
			if (Event.Type.ToLower() == "sessions")
			{
                App.Logger.TrackPage(AppPage.Sessions.ToString());

                MessagingService.Current.SendMessage(MessageKeys.NavigateToSessionList);
			}
            else if(Event.Type.ToLower() == "lunch")
            {
                App.Logger.TrackPage(AppPage.LunchLocations.ToString());

                await NavigationService.PushAsync(Navigation, new LunchLocationsPage());
            }
        }
    }
}

