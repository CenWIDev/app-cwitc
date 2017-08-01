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
//using 

namespace CWITC.Shared.DataStore.Firebase
{
    public abstract class ReadonlyStore<T> : BaseStore<T> where T: IBaseDataObject
    {
        public override Task<bool> RemoveAsync(T item)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> UpdateAsync(T item)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> InsertAsync(T item)
        {
            throw new NotSupportedException();
        }
    }

    public abstract class BaseStore<T> : IBaseStore<T>
        where T : IBaseDataObject
    {
        DatabaseReference entityNode;

        bool initialized = false;

        public BaseStore()
        {
        }

        public abstract string Identifier { get; }

        public virtual Task<T> GetItemAsync(string id)
        {
            if (!initialized) InitializeStore();

            throw new NotImplementedException();
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
                    getData.SetResult(null);
                }
            });

            return getData.Task;
        }

        public virtual Task InitializeStore()
        {
            var rootNode = global::Firebase.Database.Database.DefaultInstance.GetRootReference();
            entityNode = GetEntityNode(rootNode);

            entityNode.KeepSynced(true);
            initialized = true;
            return Task.CompletedTask;
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

        protected virtual DatabaseReference GetEntityNode(DatabaseReference rootNode)
        {
            return rootNode.GetChild(Identifier);
        }

        protected void ReloadEntityNode()
        {
            InitializeStore().Wait();
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