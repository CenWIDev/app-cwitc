using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using Newtonsoft.Json.Linq;

namespace CWITC.Shared.DataStore
{
	public abstract partial class BaseStore<T> : IReadonlyStore<T>
		where T : IBaseDataObject
	{
		public virtual Task InitializeStore() => Task.CompletedTask;

		public virtual Task<T> GetItemAsync(string id) => Task.FromResult(default(T));

		public virtual Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh) => Task.FromResult(new List<T>().AsEnumerable());

		public virtual Task<bool> SyncAsync() => Task.FromResult(true);
	}
}