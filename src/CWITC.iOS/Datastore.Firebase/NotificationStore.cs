using System;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using CWITC.iOS.DataStore.Firebase;

[assembly: Dependency(typeof(NotificationStore))]
namespace CWITC.iOS.DataStore.Firebase
{
    public class NotificationStore  : BaseStore<Notification>, INotificationStore
    {
        public override string Identifier => "notifications";

        public NotificationStore()
        {
        }

        public async Task<Notification> GetLatestNotification()
        {
            return (await GetItemsAsync())?.LastOrDefault();
        }

        public override async Task<IEnumerable<Notification>> GetItemsAsync(bool forceRefresh = false)
        {
            var items = (await base.GetItemsAsync(forceRefresh))?.Where(x => x.IsVisible)?.OrderBy(x => x.Date);

            return items?.ToList();
        }
    }
}

