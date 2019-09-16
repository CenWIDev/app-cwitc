using System;
using CWITC.DataStore.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using CWITC.Shared.DataStore;
using System.Collections.Generic;
using CWITC.Clients.Portable;

[assembly: Dependency(typeof(StoreManager))]

namespace CWITC.Shared.DataStore
{
    public class StoreManager : IStoreManager
    {
        #region IStoreManager implementation

        public async Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            var tasks = new List<Task> {
				CategoryStore.SyncAsync(),
				SessionStore.SyncAsync(),
				SpeakerStore.SyncAsync(),
				SponsorStore.SyncAsync(),
				LunchStore.SyncAsync()
            };

            if (syncUserSpecific && Settings.Current.IsLoggedIn)
            {
				tasks.Add(FavoriteStore.SyncAsync());
				tasks.Add(FeedbackStore.SyncAsync());
            }

            await Task.WhenAll(tasks);

            return true;
        }

        public bool IsInitialized { get { return true; }  }
        public Task InitializeAsync()
        {
            return Task.FromResult(true);
        }

        #endregion

        ICategoryStore categoryStore;
        public ICategoryStore CategoryStore => categoryStore ?? (categoryStore  = DependencyService.Get<ICategoryStore>());

        IFavoriteStore favoriteStore;
        public IFavoriteStore FavoriteStore => favoriteStore ?? (favoriteStore  = DependencyService.Get<IFavoriteStore>());

        IFeedbackStore feedbackStore;
        public IFeedbackStore FeedbackStore => feedbackStore ?? (feedbackStore  = DependencyService.Get<IFeedbackStore>());

        ISessionStore sessionStore;
        public ISessionStore SessionStore => sessionStore ?? (sessionStore  = DependencyService.Get<ISessionStore>());

        ISpeakerStore speakerStore;
        public ISpeakerStore SpeakerStore => speakerStore ?? (speakerStore  = DependencyService.Get<ISpeakerStore>());

        ISponsorStore sponsorStore;
        public ISponsorStore SponsorStore => sponsorStore ?? (sponsorStore  = DependencyService.Get<ISponsorStore>());

        ILunchStore lunchStore;
        public ILunchStore LunchStore => lunchStore ?? (lunchStore = DependencyService.Get<ILunchStore>());
    }
}

