using System;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.iOS.DataStore.Firebase;
using Xamarin.Forms;

[assembly: Dependency(typeof(SponsorStore))]
namespace CWITC.iOS.DataStore.Firebase
{
    public class SponsorStore : BaseStore<Sponsor>, ISponsorStore
    {
        public override string Identifier => "sponsors";
    }
}
