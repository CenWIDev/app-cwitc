using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;

using static CWITC.Clients.Portable.Constants;

namespace CWITC.Shared.DataStore
{
	public abstract class ContentfulDataStore<TEntityType, T> : BaseStore<T>
		where T : IBaseDataObject
	{
		private ContentfulClient client;

		public abstract string Identifier { get; }

		public override Task InitializeStore()
		{
			if (this.client == null)
			{
				var httpClient = new HttpClient(new MessageHandler());
				this.client = new ContentfulClient(httpClient, new Contentful.Core.Configuration.ContentfulOptions
				{
					DeliveryApiKey = ContentfulDeliveryApiKey,
					SpaceId = ContentfulSpaceKey
				});
			}

			return Task.CompletedTask;
		}

		public override async Task<T> GetItemAsync(string id)
		{
			if (client == null) await InitializeStore();

			var entry = await client.GetEntry<TEntityType>(id);

			var result = await Map(entry);

			return result;
		}

		public override async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
		{
			try
			{
				if (client == null) await InitializeStore();

				var entries = (await client.GetEntriesByType(this.Identifier, GetYearFilter()))
					?.ToList();

				List<T> results = new List<T>();
				foreach (var e in entries)
				{
					var r = await Map(e);
					results.Add(r);
				}

				return results;
			}
			catch(Exception ex)
			{
				return new List<T>();
			}
		}

		protected abstract Task<T> Map(TEntityType entity);

		protected virtual Contentful.Core.Search.QueryBuilder<TEntityType> GetYearFilter()
		{
			return new Contentful.Core.Search.QueryBuilder<TEntityType>();
		}

		private class MessageHandler : HttpClientHandler
		{
			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				var resp = await base.SendAsync(request, cancellationToken);

				//var json = await resp.Content.ReadAsStringAsync();

				return resp;
			}
		}
	}
}
