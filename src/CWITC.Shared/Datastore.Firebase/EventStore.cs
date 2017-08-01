using System;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore.Firebase;
using CWITC.Shared.DataStore.Firebase;
using Xamarin.Forms;

[assembly: Dependency(typeof(EventStore))]
namespace CWITC.Shared.DataStore.Firebase
{
    public class EventStore : BaseStore<FeaturedEvent>, IEventStore
    {
        public override string Identifier => "featured_events";
    }
}
