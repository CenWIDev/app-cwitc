using System;
using CWITC.DataObjects;
using System.Linq;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore;
using CWITC.Shared.DataStore;
using Xamarin.Forms;

[assembly: Dependency(typeof(EventStore))]
namespace CWITC.Shared.DataStore
{
    public class EventStore : BaseStore<FeaturedEvent>, IEventStore
    {
        public override string Identifier => "featured_events";

        public override async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<FeaturedEvent>> GetItemsAsync(bool forceRefresh = false)
        {
            var items = (await base.GetItemsAsync(forceRefresh))?.ToList();

            var sponsor = DependencyService.Get<ISponsorStore>();
			foreach(var item in items){
                if(!string.IsNullOrEmpty(item.SponsorId))
                {
                    item.Sponsor = await sponsor.GetItemAsync(item.SponsorId);
                }
            }

            return items;
        }
    }
}
