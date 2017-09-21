using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using FormsToolkit;
using MvvmHelpers;
using Xamarin.Forms;

namespace CWITC.Clients.Portable
{
    public class LunchLocationsViewModel : ViewModelBase
    {
        bool noLocationsFound;
        public bool NoLocationsFound
        {
            get { return noLocationsFound; }
            set { SetProperty(ref noLocationsFound, value); }
        }

        string noLocationsFoundMessage;
        public string NoLocationsFoundMessage
        {
            get { return noLocationsFoundMessage; }
            set { SetProperty(ref noLocationsFoundMessage, value); }
        }

        public ObservableRangeCollection<LunchLocation> Locations { get; } = new ObservableRangeCollection<LunchLocation>();

        public ObservableRangeCollection<ILocationViewModel> PinLocations { get; } = new ObservableRangeCollection<ILocationViewModel>();

        ICommand loadLocationsCommand;
        public ICommand LoadLocationsCommand =>
            loadLocationsCommand ?? (loadLocationsCommand = new Command(async () => await ExeucteLoadLocationsCommand(true)));

        async Task ExeucteLoadLocationsCommand(bool force)
        {
            if (IsBusy)
                return;

            bool didLoadLocations = false;

            try
            {
                IsBusy = true;

                var items = await StoreManager.LunchStore.GetItemsAsync(force);

                Locations.ReplaceRange(items);
                PinLocations.ReplaceRange(items.Select(x => new LunchLocationPinViewModel
                {
                    Title = x.Name,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Description = x.Address1,
                    Location = x
                }));

                didLoadLocations = true;
            }
            catch (Exception ex)
            {
                Logger.Report(ex, "Method", "ExecuteLoadEventsAsync");
                MessagingService.Current.SendMessage(MessageKeys.Error, ex);
            }
            finally
            {
                IsBusy = false;
            }

            if (!didLoadLocations)
            {
                NoLocationsFound = true;
                NoLocationsFoundMessage = "No Lunch Locations Available";
            }
        }

        ICommand forceRefreshCommand;
        public ICommand ForceRefreshCommand =>
            forceRefreshCommand ?? (forceRefreshCommand = new Command(async () => await ExecuteForceRefreshCommandAsync()));

        async Task ExecuteForceRefreshCommandAsync()
        {
            await ExeucteLoadLocationsCommand(true);
        }

        public LunchLocationsViewModel(INavigation navigation) : base(navigation)
        {
        }

        public class LunchLocationPinViewModel : ILocationViewModel
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public ICommand Command { get; set; }

            public LunchLocation Location { get; set; }
        }
    }
}
