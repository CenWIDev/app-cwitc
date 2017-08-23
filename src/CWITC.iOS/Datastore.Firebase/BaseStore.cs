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

namespace CWITC.Shared.DataStore.Firebase
{
    public abstract partial class BaseStore<T> : IBaseStore<T>
        where T : IBaseDataObject
    {
        DatabaseReference entityNode;

		public virtual Task InitializeStore()
		{
			var rootNode = global::Firebase.Database.Database.DefaultInstance.GetRootReference();
			entityNode = GetEntityNode(rootNode);

			entityNode.KeepSynced(true);
			initialized = true;
			return Task.CompletedTask;
		}

        public virtual Task<System.Collections.Generic.IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            if (!initialized) InitializeStore();

            TaskCompletionSource<IEnumerable<T>> getData = new TaskCompletionSource<IEnumerable<T>>();
            var query = entityNode.GetQueryOrderedByPriority();
            query.ObserveSingleEvent(
                DataEventType.Value, (DataSnapshot snapshot) =>
            {
                var values = snapshot.GetValue() as NSArray;

                if (values != null)
                {
                    List<T> items = new List<T>();
                    for (nuint i = 0; i < values.Count; i++)
                    {
                        var value = values.GetItem<NSDictionary>(i);
                        NSError error;
                        var data = NSJsonSerialization.Serialize(value, NSJsonWritingOptions.PrettyPrinted, out error);
                        //data.
                        var item = JsonConvert.DeserializeObject<T>(data.ToString());
                        items.Add(item);
                    }
                    //values.gva
                    getData.SetResult(items);
                }
                else
                {
                    getData.SetResult(new List<T>());
                }
            });

            return getData.Task;
        }

        protected virtual DatabaseReference GetEntityNode(DatabaseReference rootNode)
        {
            return rootNode.GetChild(Identifier);
        }

        async Task<bool> SaveValues(NSArray data)
        {
            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
            entityNode.SetValue(data,
                                (NSError error, DatabaseReference reference) =>
            {
                if (error != null) task.SetResult(false);

                task.SetResult(true);
            });

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

        NSArray GetArray(IEnumerable<T> existingItems)
        {
            var data = new NSMutableArray();
            foreach (var existingItem in existingItems)
            {
                data.Add(GetDictionary(existingItem));
            }
            return data;
        }
    }
}