using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CWITC.DataObjects;

namespace CWITC.DataStore.Abstractions
{
	public interface IReadonlyStore<T>
	{
		Task InitializeStore();
		Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
		Task<T> GetItemAsync(string id);
		Task<bool> SyncAsync();
	}

	public interface IBaseStore<T> : IReadonlyStore<T>
	{
		Task<bool> InsertAsync(T item);
		Task<bool> UpdateAsync(T item);
		Task<bool> RemoveAsync(T item);
	}
}

