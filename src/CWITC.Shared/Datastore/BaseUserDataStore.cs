using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using FormsToolkit;
using Xamarin.Forms;

namespace CWITC.Shared.DataStore
{
	public abstract class BaseUserDataStore<T> : IBaseStore<T> where T : IBaseDataObject
    {
		public abstract string Identifier { get; }

		public System.Threading.Tasks.Task InitializeStore()
		{
			MessagingService.Current.Subscribe(MessageKeys.LoggedIn, (m) =>
			{
				// todo: reload user data
				//ReloadEntityNode();
			});

			return Task.CompletedTask;
		}

		public Task<T> GetItemAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
		{
			throw new NotImplementedException();
		}

		public Task<bool> InsertAsync(T item)
		{
			throw new NotImplementedException();
		}

		public Task<bool> RemoveAsync(T item)
		{
			throw new NotImplementedException();
		}

		public Task<bool> SyncAsync()
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateAsync(T item)
		{
			throw new NotImplementedException();
		}
	}
}