using System;
using CWITC.DataStore.Abstractions;
using Xamarin.Forms;
using System.Threading.Tasks;
using CWITC.DataObjects;
using System.Windows.Input;
using Plugin.Share;
using FormsToolkit;
using Plugin.Share.Abstractions;

namespace CWITC.Clients.Portable
{
    public class SessionDetailsViewModel : ViewModelBase
    {
        Session session;
        public Session Session
        {
            get { return session; }
            set { SetProperty(ref session, value); }
        }

       
        public SessionDetailsViewModel(INavigation navigation, Session session) : base(navigation)
        {
            Session = session;
        }

        Speaker selectedSpeaker;
        public Speaker SelectedSpeaker
        {
            get { return selectedSpeaker; }
            set
            {
                selectedSpeaker = value;
                OnPropertyChanged();
                if (selectedSpeaker == null)
                    return;

                MessagingService.Current.SendMessage(MessageKeys.NavigateToSpeaker, selectedSpeaker);

                SelectedSpeaker = null;
            }
        }


        ICommand  favoriteCommand;
        public ICommand FavoriteCommand =>
        favoriteCommand ?? (favoriteCommand = new Command(async () => await ExecuteFavoriteCommandAsync())); 

        async Task ExecuteFavoriteCommandAsync()
        {
            await FavoriteService.ToggleFavorite(Session);

        }

        ICommand  shareCommand;
        public ICommand ShareCommand =>
            shareCommand ?? (shareCommand = new Command(async () => await ExecuteShareCommandAsync())); 

        async Task ExecuteShareCommandAsync()
        {
            Logger.Track(EvolveLoggerKeys.Share, "Title", Session.Title);
            await CrossShare.Current.Share(
                new ShareMessage { Title = "Share" },
                new ShareOptions { });
        }

        ICommand  loadSessionCommand;
        public ICommand LoadSessionCommand =>
            loadSessionCommand ?? (loadSessionCommand = new Command(async () => await ExecuteLoadSessionCommandAsync())); 

        public async Task ExecuteLoadSessionCommandAsync()
        {

            if(IsBusy)
                return;

            try 
            {
                

                IsBusy = true;

                Session.FeedbackLeft = await StoreManager.FeedbackStore.LeftFeedback(Session);


            } 
            catch (Exception ex) 
            {
                Logger.Report(ex, "Method", "ExecuteLoadSessionCommandAsync");
                MessagingService.Current.SendMessage(MessageKeys.Error, ex);
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}