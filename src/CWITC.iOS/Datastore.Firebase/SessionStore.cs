using CWITC.DataStore.Abstractions;
using CWITC.DataObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using CWITC.iOS.DataStore.Firebase;

[assembly: Dependency(typeof(SessionStore))]
namespace CWITC.iOS.DataStore.Firebase
{
    public class SessionStore : ReadonlyStore<Session>, ISessionStore
    {
        public override Task<Session> GetItemAsync(string id)
        {
            return base.GetItemAsync(id);
        }

        public override async Task<IEnumerable<Session>> GetItemsAsync(bool forceRefresh = false)
        {
            var sessions = await base.GetItemsAsync(forceRefresh);

            if (sessions != null)
            {
                var favStore = DependencyService.Get<IFavoriteStore>();
                await favStore.GetItemsAsync(true).ConfigureAwait(false);//pull latest

                foreach (var session in sessions)
                {
                    var isFav = await favStore.IsFavorite(session.Id).ConfigureAwait(false);
                    session.IsFavorite = isFav;

                    //session.spe
                }
            }

            return sessions ?? new List<Session>();
        }

        public async Task<IEnumerable<Session>> GetSpeakerSessionsAsync(string speakerId)
        {
            await InitializeStore().ConfigureAwait(false);

            var speakers = await GetItemsAsync().ConfigureAwait(false);

            return speakers.Where(s => s.Speakers != null && s.Speakers.Any(speak => speak.Id == speakerId))
                .OrderBy(s => s.StartTimeOrderBy);
        }

        public async Task<IEnumerable<Session>> GetNextSessions()
        {
            var date = DateTime.UtcNow.AddMinutes(-30);//about to start in next 30

            var sessions = await GetItemsAsync().ConfigureAwait(false);

            var result = sessions.Where(s => s.StartTimeOrderBy > date && s.IsFavorite).Take(2);

            var enumerable = result as Session[] ?? result.ToArray();
            return enumerable.Any() ? enumerable : null;
        }

        public async Task<Session> GetAppIndexSession(string id)
        {
            await InitializeStore().ConfigureAwait(false);

            var sessions = (await GetItemsAsync(true)).Where(s => s.Id == id || s.RemoteId == id).ToList();

            if (sessions == null || sessions.Count == 0)
                return null;

            return sessions[0];
        }

        public override string Identifier => "sessions";
    }
}