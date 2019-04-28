using System;

using Xamarin.Forms;
using CWITC.DataObjects;
using System.Windows.Input;
using Plugin.ExternalMaps;
using MvvmHelpers;
using FormsToolkit;
using System.Threading.Tasks;

namespace CWITC.Clients.Portable
{
    public class EventDetailsViewModel : ViewModelBase
    {
        public FeaturedEvent Event { get; set; }

        public ObservableRangeCollection<Sponsor> Sponsors { get; set; }

        public EventDetailsViewModel(INavigation navigation, FeaturedEvent e) : base(navigation)
        {
            Event = e;
            Sponsors = new ObservableRangeCollection<Sponsor>();
            if (e.Sponsor != null)
                Sponsors.Add(e.Sponsor);

            switch(Event.Type)
            {
                case "sessions":
                case "lunch" :
                case "keynote" :
                    HasExtraInfo = true;
                    break;
                default:
                    HasExtraInfo = false;
                    break;
            }
        }

        ICommand  loadEventDetailsCommand;
        public ICommand LoadEventDetailsCommand =>
            loadEventDetailsCommand ?? (loadEventDetailsCommand = new Command(() => ExecuteLoadEventDetailsCommand())); 

        void ExecuteLoadEventDetailsCommand()
        {

            if(IsBusy)
                return;

            try 
            {
                IsBusy = true;
            } 
            catch (Exception ex) 
            {
                Logger.Report(ex, "Method", "ExecuteLoadEventDetailsCommandAsync");
                MessagingService.Current.SendMessage(MessageKeys.Error, ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool HasExtraInfo { get; private set; }

        Sponsor selectedSponsor;
        public Sponsor SelectedSponsor
        {
            get { return selectedSponsor; }
            set
            {
                selectedSponsor = value;
                OnPropertyChanged();
                if (selectedSponsor == null)
                    return;

                MessagingService.Current.SendMessage(MessageKeys.NavigateToSponsor, selectedSponsor);

                SelectedSponsor = null;
            }
        }

    }
}


