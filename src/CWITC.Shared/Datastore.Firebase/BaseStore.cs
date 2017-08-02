using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;

namespace CWITC.Shared.DataStore.Firebase
{
    public abstract partial class BaseStore<T> : IBaseStore<T>
        where T : IBaseDataObject
    {
		bool initialized = false;

		public abstract string Identifier { get; }

		public virtual Task<T> GetItemAsync(string id)
		{
			if (!initialized) InitializeStore();

			throw new NotImplementedException();
		}

		public virtual async Task<bool> InsertAsync(T item)
		{
			if (!initialized) await InitializeStore();

			TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();

			var existingItems = (await GetItemsAsync(true))?.ToList() ?? new List<T>();
			existingItems.Add(item);

			return await SaveValues(GetArray(existingItems));
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

				return await SaveValues(GetArray(existingItems));
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

				return await SaveValues(GetArray(existingItems));
			}

			return false;
		}

		protected void ReloadEntityNode()
		{
			InitializeStore().Wait();
		}
    }
}
