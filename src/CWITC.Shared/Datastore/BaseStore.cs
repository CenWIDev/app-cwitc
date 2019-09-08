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
		// all data for the app is in a single json file
		// i know, gross. deal with it (⌐■_■)
		static JObject LoadedData;

		bool initialized => LoadedData != null;

		public abstract string Identifier { get; }

		public virtual async Task InitializeStore()
		{
			if (LoadedData == null)
				LoadedData = await this.GetDataFile();
		}

		public virtual async Task<T> GetItemAsync(string id)
		{
			if (!initialized) await this.InitializeStore();

			return (await GetItemsAsync(false)).FirstOrDefault(x => x.Id == id);
		}

		public virtual async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh)
		{
			if (!initialized) await InitializeStore();
			if (forceRefresh) LoadedData = await this.GetDataFile(forceRefresh);

			try
			{
				if (LoadedData.ContainsKey(this.Identifier))
				{
					var section = LoadedData[this.Identifier];

					return section.ToObject<IEnumerable<T>>();
				}

				return new List<T>();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
				return new List<T>();
			}
		}

		public virtual async Task<bool> SyncAsync()
		{
			// todo: ??
			LoadedData = await GetDataFile(true);

			return true;
		}

		async Task<JObject> GetDataFile(bool forceDownload = false)
		{
			return new JObject();
		}
	}
}