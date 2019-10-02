using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using FormsToolkit;
using Newtonsoft.Json.Linq;

namespace CWITC.Shared.DataStore
{
	public abstract partial class BaseUserDataStore<T> : IBaseStore<T> where T : IBaseDataObject
	{
		bool initialized = false;

		public abstract string Identifier { get; }

		public virtual async Task<T> GetItemAsync(string id)
		{
			if (!initialized) await InitializeStore();

			return (await GetItemsAsync(false)).FirstOrDefault(x => x.Id == id);
		}

		public virtual async Task<bool> InsertAsync(T item)
		{
			if (!initialized) await InitializeStore();

			var existingItems = (await GetItemsAsync(true))?.ToList() ?? new List<T>();
			existingItems.Add(item);

			return await SaveValues(GetSaveValue(existingItems));
		}

		public virtual async Task<bool> RemoveAsync(T item)
		{
			if (!initialized) await InitializeStore();

			TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();

			var existingItems = (await GetItemsAsync(true))?.ToList() ?? new List<T>();
			var foundItem = existingItems.FirstOrDefault((x) => x.Id == item.Id);
			if (foundItem != null)
			{
				var index = existingItems.IndexOf(foundItem);

				existingItems.RemoveAt(index);

				return await SaveValues(GetSaveValue(existingItems));
			}
			return false;
		}

		public virtual Task<bool> SyncAsync()
		{
			// todo: ??

			// nothing to do here for firebase, its automagical
			return Task.FromResult(true);
		}

		public virtual async Task<bool> UpdateAsync(T item)
		{
			if (!initialized) await InitializeStore();

			TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();

			var existingItems = (await GetItemsAsync(true))?.ToList() ?? new List<T>();
			var foundItem = existingItems.FirstOrDefault((x) => x.Id == item.Id);
			if (foundItem != null)
			{
				var index = existingItems.IndexOf(foundItem);

				existingItems[index] = item;

				return await SaveValues(GetSaveValue(existingItems));
			}

			return false;
		}

		protected virtual IEnumerable<T> ParseJsonResults(JToken json)
		{
			return (IEnumerable<T>)json.ToObject(typeof(List<T>));
		}

		protected void ReloadEntityNode()
		{
			InitializeStore().Wait();
		}

		private void OnStoreInitialized()
		{
			MessagingService.Current.Subscribe(MessageKeys.LoggedIn, (m) =>
			{
				ReloadEntityNode();
			});
		}
	}
}