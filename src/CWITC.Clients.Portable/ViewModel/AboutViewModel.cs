using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FormsToolkit;
using MvvmHelpers;
using Xamarin.Forms;
using Plugin.Share;

namespace CWITC.Clients.Portable
{
    public class AboutViewModel : SettingsViewModel
    {
        public ObservableRangeCollection<Grouping<string, MenuItem>> MenuItems { get; }
        public ObservableRangeCollection<MenuItem> InfoItems { get; } = new ObservableRangeCollection<MenuItem>();
        public ObservableRangeCollection<MenuItem> AccountItems { get; } = new ObservableRangeCollection<MenuItem>();

        MenuItem syncItem;
        MenuItem accountItem;
        public AboutViewModel()
        {
            AboutItems.Clear();
            AboutItems.Add(new MenuItem { Name = "About this app", Icon = "icon_venue.png" });

            InfoItems.AddRange(new[]
                {
                    new MenuItem { Name = "Sponsors", Icon = "icon_venue.png", Parameter="sponsors"},
                    new MenuItem { Name = "Event Map", Icon = "icon_venue.png", Parameter = "floor-maps"},
                    new MenuItem { Name = "Lunch Locations", Icon = "ic_restaurant_menu.png", Parameter = "lunch-locations"},
                    new MenuItem { Name = "Venue", Icon = "icon_venue.png", Parameter = "venue"},
                });

            accountItem = new MenuItem
            {
                Name = "Logged in as:"
            };

            syncItem = new MenuItem
            {
                Name = "Last Sync:"
            };

            UpdateItems();

            AccountItems.Add(accountItem);
            AccountItems.Add(syncItem);

            //This will be triggered wen 
            Settings.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "Email" || e.PropertyName == "LastSync" || e.PropertyName == "PushNotificationsEnabled")
                    {
                        UpdateItems();
                        OnPropertyChanged("AccountItems");
                    }
                };
        }

        public void UpdateItems()
        {
            syncItem.Subtitle = LastSyncDisplay;
            accountItem.Subtitle = Settings.Current.IsLoggedIn ? Settings.Current.UserDisplayName : "Not signed in";
        }

    }
}

