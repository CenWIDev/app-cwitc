using System;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared;
using CWITC.Shared.DataStore.Firebase;
using Xamarin.Forms;

[assembly: Dependency(typeof(LunchStore))]
namespace CWITC.Shared
{
    public partial class LunchStore : BaseStore<LunchLocation>, ILunchStore
    {
        public override string Identifier => "lunch_locations";
    }
}
