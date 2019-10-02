#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using Firebase.Database;
using GoogleGson.Reflect;
using Java.Lang;
using Java.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.Json;
using JsonHelper = CWITC.Droid.JavaJsonHelperExtensions;

namespace CWITC.Shared.DataStore
{
	public abstract partial class BaseUserDataStore<T> : IBaseStore<T> where T : IBaseDataObject
	{
		TaskCompletionSource<bool> saveTask;
		DatabaseReference entityNode;

		string UserId => Firebase.Auth.FirebaseAuth.Instance.CurrentUser.Uid;

		public virtual Task<System.Collections.Generic.IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
		{
			if (!initialized) InitializeStore();

			var getData = new TaskCompletionSource<IEnumerable<T>>();

			var query = entityNode.OrderByPriority();
			query
				.AddListenerForSingleValueEvent(new ValueEventListenerCallback(getData, this.ParseJsonResults));

			return getData.Task;
		}

		public virtual Task InitializeStore()
		{
			var rootNode = FirebaseDatabase.Instance.Reference;

			entityNode = rootNode.Child("2019")
				.Child(UserId)
				.Child(Identifier);

			entityNode.KeepSynced(true);
			initialized = true;
			return Task.CompletedTask;
		}

		Task<bool> SaveValues(Java.Lang.Object data)
		{
			saveTask = new TaskCompletionSource<bool>();

			entityNode.SetValue(data,
								new SaveCompletionListener(saveTask) as DatabaseReference.ICompletionListener);

			return saveTask.Task;
		}

		IMap GetDictionary(T item)
		{
			string jsonString = JsonConvert.SerializeObject(item);
			var jsonObject = new JSONObject(jsonString);

			return JsonHelper.ToJavaMap(jsonObject);
		}

		protected abstract Java.Lang.Object GetSaveValue(IEnumerable<T> existingItems);

		class ValueEventListenerCallback : Java.Lang.Object, IValueEventListener
		{
			TaskCompletionSource<bool> saveTask;
			TaskCompletionSource<IEnumerable<T>> getTask;
			private readonly Func<JToken, IEnumerable<T>> parseResultsFunc;

			public ValueEventListenerCallback(TaskCompletionSource<bool> saveTask)
			{
				this.saveTask = saveTask;
			}

			public ValueEventListenerCallback(TaskCompletionSource<IEnumerable<T>> getTask, Func<JToken, IEnumerable<T>> parseJsonResults)
			{
				this.getTask = getTask;
				this.parseResultsFunc = parseJsonResults;
			}

			void IValueEventListener.OnCancelled(DatabaseError error)
			{
				this.getTask.TrySetCanceled();
			}

			void IValueEventListener.OnDataChange(DataSnapshot snapshot)
			{
				var values = (snapshot.Value as ArrayList)?.ToArray();

				if (values != null)
				{
					List<T> items = new List<T>();
					var data = new GoogleGson.Gson().ToJson(values);
					var json = JToken.Parse(data);
					var parsedresults = this.parseResultsFunc?.Invoke(json);
					if (parsedresults != null)
					{
						items.AddRange(parsedresults);
					}

					getTask.TrySetResult(items);
				}
				else
				{
					getTask.TrySetResult(new List<T>());
				}
			}
		}

		class SaveCompletionListener : Java.Lang.Object, DatabaseReference.ICompletionListener
		{
			TaskCompletionSource<bool> saveTask;

			public SaveCompletionListener(TaskCompletionSource<bool> saveTask)
			{
				this.saveTask = saveTask;
			}

			void DatabaseReference.ICompletionListener.OnComplete(DatabaseError error, DatabaseReference @ref)
			{
				if (error == null)
					saveTask.TrySetResult(true);
				else
					saveTask.TrySetResult(false);
			}
		}
	}
}
#endif