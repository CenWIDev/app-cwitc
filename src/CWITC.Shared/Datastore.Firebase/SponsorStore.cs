using System;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore.Firebase;
using CWITC.Shared.DataStore.Firebase;
using Xamarin.Forms;

[assembly: Dependency(typeof(SponsorStore))]
namespace CWITC.Shared.DataStore.Firebase
{
    public class SponsorStore : BaseStore<Sponsor>, ISponsorStore
    {
        public override string Identifier => "sponsors";
    }
}
