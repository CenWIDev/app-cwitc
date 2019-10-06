using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CWITC.Clients.Portable;
using FormsToolkit;
using CWITC.DataObjects;

namespace CWITC.Clients.UI
{
    public partial class FeedPage : ContentPage
    {
        FeedViewModel ViewModel => vm ?? (vm = BindingContext as FeedViewModel);
        FeedViewModel vm;
        DateTime favoritesTime;
        string loggedIn;
        public FeedPage()
        {
            InitializeComponent();
            loggedIn = Settings.Current.Email;
            BindingContext = new FeedViewModel();

            favoritesTime = Settings.Current.LastFavoriteTime;

            ViewModel.Sessions.CollectionChanged += (sender, e) => 
                {
                    var adjust = Device.RuntimePlatform != "Android" ? 1 : -ViewModel.Sessions.Count + 1;
                    ListViewSessions.HeightRequest = (ViewModel.Sessions.Count * ListViewSessions.RowHeight) - adjust;
                };

            ListViewSessions.ItemTapped += (sender, e) => ListViewSessions.SelectedItem = null;
            ListViewSessions.ItemSelected += async (sender, e) => 
                {
                    var session = ListViewSessions.SelectedItem as Session;
                    if(session == null)
                        return;
                    var sessionDetails = new SessionDetailsPage(session);

                    App.Logger.TrackPage(AppPage.Session.ToString(), session.Title);
                    await NavigationService.PushAsync(Navigation, sessionDetails);
                    ListViewSessions.SelectedItem = null;
                }; 
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdatePage();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        bool firstLoad = true;
        private void UpdatePage()
        {
            bool forceRefresh = (DateTime.UtcNow > (ViewModel?.NextForceRefresh ?? DateTime.UtcNow)) ||
                    loggedIn != Settings.Current.Email;

            loggedIn = Settings.Current.Email;
            if (forceRefresh)
            {
                ViewModel.RefreshCommand.Execute(null);
                favoritesTime = Settings.Current.LastFavoriteTime;
            }
            else
            {
                if ((firstLoad && ViewModel.Sessions.Count == 0) || favoritesTime != Settings.Current.LastFavoriteTime)
                {
                    if (firstLoad)
                        Settings.Current.LastFavoriteTime = DateTime.UtcNow;
                    
                    firstLoad = false;
                    favoritesTime = Settings.Current.LastFavoriteTime;
                    ViewModel.LoadSessionsCommand.Execute(null);
                }
            }

        }


        public void OnResume()
        {
            UpdatePage();
        }

    }
}

