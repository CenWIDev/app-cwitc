#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using Firebase.Database;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CWITC.Shared.DataStore
{
	public abstract partial class BaseUserDataStore<T>
	{
		DatabaseReference entityNode;

		string UserId => Firebase.Auth.Auth.DefaultInstance.CurrentUser.Uid;

		public virtual Task InitializeStore()
		{
			try
			{
				var rootNode = global::Firebase.Database.Database.DefaultInstance.GetRootReference();

				entityNode = rootNode.GetChild("2019")
					.GetChild(UserId)
					.GetChild(Identifier);

				entityNode.KeepSynced(true);
				initialized = true;

				this.OnStoreInitialized();
			}
			catch(Exception ex)
			{
				throw ex;
			}

			return Task.CompletedTask;
		}

		public virtual async Task<System.Collections.Generic.IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
		{
			try
			{
				if (!initialized) await InitializeStore();

				TaskCompletionSource<IEnumerable<T>> getData = new TaskCompletionSource<IEnumerable<T>>();
				var query = entityNode.GetQueryOrderedByPriority();
				query.ObserveSingleEvent(
					DataEventType.Value, (DataSnapshot snapshot) =>
					{
						var values = snapshot.GetValue() as NSDictionary;

						if (values != null)
						{
							List<T> items = new List<T>();

							var data = NSJsonSerialization.Serialize(values, NSJsonWritingOptions.PrettyPrinted, out NSError toJsonError);

							var json = JToken.Parse(data.ToString());
							var parsedResults = this.ParseJsonResults(json);

							if (parsedResults != null)
							{
								items.AddRange(parsedResults);
							}

							getData.SetResult(items);
						}
						else
						{
							getData.SetResult(new List<T>());
						}
					});

				return await getData.Task;
			}
			catch (Exception ex)
			{ 
			 throw ex;
			}
		}

		async Task<bool> SaveValues(NSDictionary data)
		{
			TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
			DatabaseReferenceCompletionHandler onComplete = (NSError error, DatabaseReference reference) =>
			{
				if (error != null)
				{
					task.SetResult(false);
					return;
				}

				task.SetResult(true);
			};

			entityNode.SetValue(value: data, completionHandler: onComplete);

			return await task.Task;
		}

		NSDictionary GetDictionary(object item)
		{
			var json = JsonConvert.SerializeObject(item);

			NSString jsonString = new NSString(json);
			//NSDictionary.from
			var data = NSData.FromString(json, NSStringEncoding.UTF8);

			NSError error;
			NSDictionary jsonDic = (NSDictionary)NSJsonSerialization.Deserialize(data, NSJsonReadingOptions.AllowFragments, out error);

			return jsonDic;
		}

		protected abstract NSDictionary GetSaveValue(IEnumerable<T> existingItems);
	}
}
#endif
