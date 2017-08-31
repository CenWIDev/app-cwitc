using Xamarin.Forms;
using CWITC.Clients.UI;
using FormsToolkit;
using CWITC.Clients.Portable;
using CWITC.DataStore.Abstractions;

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
                        case AppPage.Events:
                            NavigateAsync(AppPage.Events);
                            await CurrentPage.Navigation.PopToRootAsync();
                            break;
                        case AppPage.Session:
                            NavigateAsync(AppPage.Sessions);
                            var session = await DependencyService.Get<ISessionStore>().GetAppIndexSession(p.Id);
                            if (session == null)
                                break;
                            await CurrentPage.Navigation.PushAsync(new SessionDetailsPage(session));
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
                    App.Logger.TrackPage(AppPage.Events.ToString());
                    break;
                case 3:
                    App.Logger.TrackPage(AppPage.Information.ToString());
                    break;
            }
        }

        public void NavigateAsync(AppPage menuId)
        {
            switch ((int)menuId)
            {
                case (int)AppPage.Feed: CurrentPage = Children[0]; break;
                case (int)AppPage.Sessions: CurrentPage = Children[1]; break;
                case (int)AppPage.Events: CurrentPage = Children[2]; break;
                case (int)AppPage.Information: CurrentPage = Children[3]; break;
                case (int)AppPage.Notification: CurrentPage = Children[0]; break;
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


