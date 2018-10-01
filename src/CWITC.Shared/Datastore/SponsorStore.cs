using System;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore;
using CWITC.Shared.DataStore;
using Xamarin.Forms;

[assembly: Dependency(typeof(SponsorStore))]
namespace CWITC.Shared.DataStore
{
    public class SponsorStore : BaseStore<Sponsor>, ISponsorStore
    {
        public override string Identifier => "sponsors";
    }
}
