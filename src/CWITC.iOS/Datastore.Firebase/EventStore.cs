using System;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.iOS.DataStore.Firebase;
using Xamarin.Forms;

[assembly: Dependency(typeof(EventStore))]
namespace CWITC.iOS.DataStore.Firebase
{
    public class EventStore : BaseStore<FeaturedEvent>, IEventStore
    {
        public override string Identifier => "featured_events";
    }
}
