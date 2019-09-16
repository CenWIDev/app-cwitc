using CWITC.DataStore.Abstractions;
using CWITC.DataObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using CWITC.Shared.DataStore;
using CWITC.Clients.Portable;
using Contentful.Core.Models;
using Contentful.Core.Search;

[assembly: Dependency(typeof(SessionStore))]
namespace CWITC.Shared.DataStore
{
	public class SessionStore : ContentfulDataStore<SessionEntity, Session>, ISessionStore
	{
		public async Task<IEnumerable<Session>> GetSpeakerSessionsAsync(string speakerId)
		{
			await InitializeStore()
				.ConfigureAwait(false);

			var sessions = await GetItemsAsync()
				.ConfigureAwait(false);

			return sessions
				.Where(s => s.Speakers != null && s.Speakers.Any(speak => speak.Id == speakerId)).OrderBy(s => s.StartTimeOrderBy);
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

			var sessions = (await GetItemsAsync(true)).Where(s => s.Id == id).ToList();

			if (sessions == null || sessions.Count == 0)
				return null;

			return sessions[0];
		}

		protected override QueryBuilder<SessionEntity> GetYearFilter()
		{
			return new QueryBuilder<SessionEntity>()
				.FieldGreaterThan(s => s.StartTime, new DateTime(2019, 1, 1).ToString("o"));
		}

		protected override async Task<Session> Map(SessionEntity entity)
		{
			var htmlRenderer = new HtmlRenderer();
			var html = await htmlRenderer.ToHtml(entity.Description);

			return new Session
			{
				Id = entity.Sys.Id,
				Title = entity.Title,
				ShortTitle = entity.Title,
				Abstract = html,
				Room = new Room
				{
					Name = entity.Room
				},
				StartTime = entity.StartTime,
				EndTime = entity.EndTime,
				MainCategory = new Category
				{
					Id = entity.Category?.Sys.Id,
					Name = entity.Category?.Name,
					ShortName = entity.Category?.ShortName,
					Color = entity.Category?.Color
				},
				Speakers = entity.Speakers?.Select(s => new Speaker
				{
					Id = s?.Sys.Id,
					Name = s?.Name,
					Biography = s?.Biography,
					CompanyName = s?.CompanyName,
					CompanyWebsiteUrl = s?.CompanyWebsiteURL,
					//PhotoUrl = s.p
				})?.ToList()
			};
		}

		public override string Identifier => "session";
	}
}
