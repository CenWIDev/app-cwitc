﻿using Xamarin.Forms;
using CWITC.Clients.UI;
using FormsToolkit;
using CWITC.Clients.Portable;
using CWITC.DataStore.Abstractions;
using System;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace CWITC.Clients.UI
{
    public class RootPage : Xamarin.Forms.TabbedPage
	{

        public RootPage()
        {
			this.On<Xamarin.Forms.PlatformConfiguration.Android>()
				.SetToolbarPlacement(ToolbarPlacement.Bottom);

			NavigationPage.SetHasNavigationBar(this, false);
            Children.Add(new EvolveNavigationPage(new FeedPage()));
            Children.Add(new EvolveNavigationPage(new SessionsPage()));
			Children.Add(new EvolveNavigationPage(new LunchLocationsPage()));
            Children.Add(new EvolveNavigationPage(new AboutPage()));

            MessagingService.Current.Subscribe<DeepLinkPage>("DeepLinkPage", async (m, p) =>
                {
                    switch (p.Page)
                    {
                        case AppPage.Schedule:
						    NavigateAsync(AppPage.Schedule);
                            await CurrentPage.Navigation.PopToRootAsync();
                            break;
                    }
                });

            MessagingService.Current.Subscribe(MessageKeys.NavigateToSessionList, m =>
            {
                CurrentPage = Children[1];
            });
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            switch (Children.IndexOf(CurrentPage))
            {
                case 0:
                    App.Logger.TrackPage(AppPage.Feed.ToString());
                    break;
                case 1:
                    App.Logger.TrackPage(AppPage.Sessions.ToString());
                    break;
                case 2:
					App.Logger.TrackPage(AppPage.LunchLocations.ToString());
                    break;
				case 3:
					App.Logger.TrackPage(AppPage.Schedule.ToString());
					break;
                case 4:
                    App.Logger.TrackPage(AppPage.Information.ToString());
                    break;
            }
        }

        public void NavigateAsync(AppPage menuId)
        {
            switch ((int)menuId)
            {
                case (int)AppPage.Feed: 
                case (int)AppPage.LunchLocations:
                    CurrentPage = Children[0]; 
                    break;
                case (int)AppPage.Sessions: CurrentPage = Children[1]; break;
                case (int)AppPage.Schedule: CurrentPage = Children[2]; break;
                case (int)AppPage.Information: CurrentPage = Children[3]; break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Settings.Current.FirstRun)
            {
                MessagingService.Current.SendMessage(MessageKeys.NavigateLogin);
            }
        }


    }
}


