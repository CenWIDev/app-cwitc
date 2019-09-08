using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CWITC.Clients.Portable;
using FormsToolkit;
using CWITC.DataObjects;

namespace CWITC.Clients.UI
{
    public partial class EventsPage : ContentPage
    {
        EventsViewModel vm;
        EventsViewModel ViewModel => vm ?? (vm = BindingContext as EventsViewModel); 

        public EventsPage()
        {
            InitializeComponent();
            BindingContext = new EventsViewModel(Navigation);

            ListViewEvents.ItemTapped += (sender, e) => ListViewEvents.SelectedItem = null;
            ListViewEvents.ItemSelected += async (sender, e) => 
                {
                    var ev = ListViewEvents.SelectedItem as FeaturedEvent;
                    if(ev == null)
                        return;
                    
                    var eventDetails = new EventDetailsPage();

                    eventDetails.Event = ev;
                    App.Logger.TrackPage(AppPage.Event.ToString(), ev.Title);
                    await NavigationService.PushAsync(Navigation, eventDetails);

                    ListViewEvents.SelectedItem = null;
                };
        }
            
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel.Events.Count == 0)
                ViewModel.LoadEventsCommand.Execute(false);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}

