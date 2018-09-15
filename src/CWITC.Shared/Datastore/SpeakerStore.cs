using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpeakerStore))]
namespace CWITC.Shared.DataStore
{
    public class SpeakerStore : BaseStore<Speaker>, ISpeakerStore
    {
        public override async Task<IEnumerable<Speaker>> GetItemsAsync(bool forceRefresh = false)
        {
            var sessionStore = DependencyService.Get<ISessionStore>();
            var sessions = await sessionStore.GetItemsAsync();

            var speakers = sessions.SelectMany(s => s.Speakers).ToList();

            return speakers;
        }

        public override async Task<Speaker> GetItemAsync(string id)
        {
            var speakers = await GetItemsAsync();
            var speaker = speakers.FirstOrDefault(s => s.Id == id);

            return speaker;
        }

        public override string Identifier => "sessions";
    }
}