using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CWITC.DataObjects;
using FormsToolkit;
using Plugin.ExternalMaps;
using Plugin.ExternalMaps.Abstractions;
using Plugin.Messaging;
using Plugin.Share;
using Xamarin.Forms;

namespace CWITC.Clients.Portable
{
    public class LunchLocationsDetailViewModel : ViewModelBase
    { 
        LunchLocation _location;
        public LunchLocation Location => _location;

        public bool CanMakePhoneCall => CrossMessaging.Current.PhoneDialer.CanMakePhoneCall && !string.IsNullOrEmpty(Location?.PhoneNumber);

		ICommand navigateCommand;
		public ICommand NavigateCommand =>
			navigateCommand ?? (navigateCommand = new Command(async () => await ExecuteNavigateCommandAsync()));

		async Task ExecuteNavigateCommandAsync()
		{
			Logger.Track(EvolveLoggerKeys.NavigateToCWITC);
			if (!await CrossExternalMaps.Current.NavigateTo(
                Location.Name,
                Location.Latitude,
                Location.Longitude,
                NavigationType.Default))
			{
				MessagingService.Current.SendMessage(MessageKeys.Message, new MessagingServiceAlert
				{
					Title = "Unable to Navigate",
					Message = "Please ensure that you have a map application installed.",
					Cancel = "OK"
				});
			}
		}

		ICommand callCommand;
		public ICommand CallCommand =>
			callCommand ?? (callCommand = new Command(ExecuteCallCommand));

		void ExecuteCallCommand()
		{
			Logger.Track(EvolveLoggerKeys.CallVenue);
			var phoneCallTask = CrossMessaging.Current.PhoneDialer;
			if (phoneCallTask.CanMakePhoneCall)
                phoneCallTask.MakePhoneCall(Location.PhoneNumber);
		}

        ICommand viewMenuCommand;
        public ICommand ViewMenuCommand =>
        viewMenuCommand ?? (viewMenuCommand = new Command(ExecuteViewMenuCommand));

        void ExecuteViewMenuCommand(object obj)
        {
            try
            {
                Xamarin.Forms.Device.OpenUri(new Uri(Location.Website));
            }
            catch
            {
                MessagingService.Current.SendMessage(MessageKeys.Message, new MessagingServiceAlert
                {
                    Title = "Unable to Open Website",
                    Message = "A website is not available for this locaiton.",
                    Cancel = "OK"
                });
            }
        }

        public LunchLocationsDetailViewModel(INavigation navigation, LunchLocation item) 
            : base(navigation)
        {
            _location = item;
        }
    }
}
