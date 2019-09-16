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
    public class SpeakerStore : ContentfulDataStore<SpeakerEntity, Speaker>, ISpeakerStore
    {
        public override async Task<IEnumerable<Speaker>> GetItemsAsync(bool forceRefresh = false)
        {
			var speakers = await base.GetItemsAsync(forceRefresh);

            var sessionStore = DependencyService.Get<ISessionStore>();
			foreach (var speaker in speakers)
				speaker.Sessions = (await sessionStore.GetSpeakerSessionsAsync(speaker.Id))?.ToList();

            return speakers;
        }

        public override async Task<Speaker> GetItemAsync(string id)
        {
			var speaker = await base.GetItemAsync(id);

			var sessionStore = DependencyService.Get<ISessionStore>();
			speaker.Sessions = (await sessionStore.GetSpeakerSessionsAsync(speaker.Id))?.ToList();

			return speaker;
        }

		protected override Task<Speaker> Map(SpeakerEntity entity)
		{
			return Task.FromResult(new Speaker
			{
				Id = entity?.Sys.Id,
				Name = entity?.Name,
				Biography = entity?.Biography,
				CompanyName = entity?.CompanyName,
				CompanyWebsiteUrl = entity?.CompanyWebsiteURL,
				PhotoUrl = !string.IsNullOrEmpty(entity.Photo?.File.Url) ?
						$"http://{entity.Photo.File.Url}" : null
			});
		}

		public override string Identifier => "speaker";
    }
}