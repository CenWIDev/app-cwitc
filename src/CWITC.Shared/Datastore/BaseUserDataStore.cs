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
	public abstract class BaseUserDataStore<T> : IBaseStore<T> where T : IBaseDataObject
	{
		//ManagementApiClient apiClient = null;
		bool initialized => false;// apiClient != null;

		public abstract string Identifier { get; }

		public System.Threading.Tasks.Task InitializeStore()
		{
			//if (apiClient == null)
			//	apiClient = new ManagementApiClient(Settings.Current.IdToken, new Uri("https://cwitc.auth0.com/api/v2"));

			MessagingService.Current.Subscribe(MessageKeys.LoggedIn, (m) =>
			{
				// todo: reload user data
				this.GetDataFile();
				//ReloadEntityNode();
			});

			return Task.CompletedTask;
		}

		public async Task<T> GetItemAsync(string id)
		{
			if (!initialized) await InitializeStore();

			return (await GetItemsAsync(false)).FirstOrDefault(x => x.Id == id);
		}

		public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
		{
			if (!initialized) await InitializeStore();

			try
			{
				var json = await GetDataFile(forceRefresh);

				return json.ToObject<IEnumerable<T>>();
			}
			catch(Exception ex)
			{
				return new List<T>();
			}
		}

		public async Task<bool> InsertAsync(T item)
		{
			if (!initialized) await InitializeStore();

			var results = (await GetItemsAsync())?.ToList() ?? new List<T>();
			results.Add(item);

			await this.SaveData(results);
			return true;
		}

		public async Task<bool> RemoveAsync(T item)
		{
			if (!initialized) await InitializeStore();

			var results = (await GetItemsAsync())?.ToList() ?? new List<T>();

			try
			{
				var found = results.First(x => x.Id == item.Id);
				results.Remove(found);
				await this.SaveData(results);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> SyncAsync()
		{
			try
			{
				if (!initialized) await InitializeStore();

				await this.GetDataFile(true);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> UpdateAsync(T item)
		{
			if (!initialized) await InitializeStore();

			var results = (await GetItemsAsync())?.ToList() ?? new List<T>();

			try
			{
				var found = results.First(x => x.Id == item.Id);
				var index = results.IndexOf(found);
				results.Remove(found);
				results.Insert(index, item);
				await this.SaveData(results);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private async Task SaveData(List<T> data)
		{
			//var user = await GetUser();

			//user.UserMetadata.GetType().GetProperty(name).SetValue(user.UserMetadata, data);

			//var properties = user.UserMetadata.GetType().GetProperties();
			//foreach(var property in properties)
			//{
			//	var name = property.Name;
			//	if(name == this.Identifier)
			//	{

			//		break;
			//	}
			//}
			//user.UserMetadata.
			//user.UserMetadata = data;

			JObject toSend = new JObject();
			toSend.Add(this.Identifier, JArray.FromObject(data));

			//await apiClient.Users.UpdateAsync(Settings.Current.UserId, new Auth0.ManagementApi.Models.UserUpdateRequest
			//{
			//	UserMetadata = toSend
			//});

			string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{this.Identifier}.json");
			File.WriteAllText(fileName, data.ToString(), System.Text.Encoding.UTF8);
			//apiClient.Users.u
		}

		private async Task<JArray> GetDataFile(bool forceDownload = false)
		{
			string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{this.Identifier}.json");

			if (!forceDownload && File.Exists(fileName))
			{
				try
				{
					var data = File.ReadAllText(fileName);

					return JArray.Parse(data);
				}
				catch { return new JArray(); }
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
				//var user = await GetUser();

				//if (user.UserMetadata == null)
				//	return new JArray();
				
				//var metadata = JArray.FromObject(user.UserMetadata);

				//if (metadata.ContainsKey(this.Identifier))
				//{
				//	var section = metadata[this.Identifier];

				//	string data = section.ToString();

				//	File.WriteAllText(fileName, data, System.Text.Encoding.UTF8);

				//	return JArray.Parse(data);
				//}
			}

			return new JArray();
		}

		//async Task<Auth0.ManagementApi.Models.User> GetUser()
		//{
		//	var user = await apiClient.Users.GetAsync(Settings.Current.UserId);
		//	return user;
		//}
	}
}