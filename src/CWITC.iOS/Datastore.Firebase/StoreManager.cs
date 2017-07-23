using System;
using CWITC.DataStore.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using CWITC.iOS.DataStore.Firebase;

[assembly: Dependency(typeof(StoreManager))]

namespace CWITC.iOS.DataStore.Firebase
{
    public class StoreManager : IStoreManager
    {
        #region IStoreManager implementation

        public Task<bool> SyncAllAsync(bool syncUserSpecific)
        {
            // todo: download all data on start
            return Task.FromResult(true);
        }

        public bool IsInitialized { get { return true; }  }
        public Task InitializeAsync()
        {
            return Task.FromResult(true);
        }

        #endregion

        public Task DropEverythingAsync()
        {
            return Task.FromResult(true);
        }

        INotificationStore notificationStore;
        public INotificationStore NotificationStore => notificationStore ?? (notificationStore  = DependencyService.Get<INotificationStore>());

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

        IEventStore eventStore;
        public IEventStore EventStore => eventStore ?? (eventStore = DependencyService.Get<IEventStore>());

        ISponsorStore sponsorStore;
        public ISponsorStore SponsorStore => sponsorStore ?? (sponsorStore  = DependencyService.Get<ISponsorStore>());

    }
}

