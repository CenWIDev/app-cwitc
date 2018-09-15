using Xamarin.Forms;
using CWITC.Clients.UI;
using FormsToolkit;
using CWITC.Clients.Portable;
using CWITC.DataStore.Abstractions;
using System;

namespace CWITC.Clients.UI
{
    public class RootPageiOS : TabbedPage
    {

        public RootPageiOS()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            Children.Add(new EvolveNavigationPage(new FeedPage()));
            Children.Add(new EvolveNavigationPage(new SessionsPage()));
            Children.Add(new EvolveNavigationPage(new EventsPage()));
            Children.Add(new EvolveNavigationPage(new AboutPage()));

            MessagingService.Current.Subscribe<DeepLinkPage>("DeepLinkPage", async (m, p) =>
                {
                    switch (p.Page)
                    {
                        case AppPage.Notification:
                            NavigateAsync(AppPage.Notification);
                            await CurrentPage.Navigation.PopToRootAsync();
                            await CurrentPage.Navigation.PushAsync(new NotificationsPage());
                            break;
                        case AppPage.Schedule:
						    NavigateAsync(AppPage.Schedule);
                            await CurrentPage.Navigation.PopToRootAsync();
                            break;
                        case AppPage.LunchLocations:
						    NavigateAsync(AppPage.LunchLocations);
                            await CurrentPage.Navigation.PopToRootAsync();
                            //await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
                            await NavigationService.PushModalAsync(
                                Navigation,
                                new EvolveModalNavigationPage(new LunchLocationsPage()),
                                true);
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
                case (int)AppPage.Notification: 
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


