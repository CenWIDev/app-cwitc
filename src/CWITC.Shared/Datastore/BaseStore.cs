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
		string year => "2017";
		string github_file_url => $"https://raw.githubusercontent.com/CenWIDev/app-cwitc/app_data/{year}.json";

		JObject loadedData;
		bool initialized => loadedData != null;

		public abstract string Identifier { get; }

		public virtual async Task InitializeStore()
		{
			if(loadedData != null)
				loadedData = await this.GetDataFile();
		}

		public virtual async Task<T> GetItemAsync(string id)
		{
			if (!initialized) await InitializeStore();

			return (await GetItemsAsync(false)).FirstOrDefault(x => x.Id == id);
		}

		public virtual async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh)
		{
			if (!initialized) await InitializeStore();

			if (this.loadedData.ContainsKey(this.Identifier))
			{
				var section = json[this.Identifier];

				return section.ToObject<IEnumerable<T>>();
			}

			return new List<T>();
		}

		public virtual async Task<bool> SyncAsync()
		{
			// todo: ??
			loadedData = await GetDataFile(true);

			return true;
		}

		private async Task<JObject> GetDataFile(bool forceDownload = false)
		{
			string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{this.year}.json");

			if (!forceDownload && File.Exists(fileName))
			{
				var data = File.ReadAllText(fileName);

				return JObject.Parse(data);
			}
			else
			{
				using (var fs = File.Create(fileName))
				{
					fs.Close();
				}
			}

			if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
			{
				using (var client = new HttpClient())
				{
					string response = await client.GetStringAsync(this.github_file_url);

					File.WriteAllText(fileName, response, System.Text.Encoding.UTF8);

					return JObject.Parse(response);
				}
			}
			else
				return new JObject();
		}
	}
}
