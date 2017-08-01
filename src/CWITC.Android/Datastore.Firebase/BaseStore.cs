using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using Firebase.Database;
using GoogleGson.Reflect;
using Java.Util;
using Newtonsoft.Json;
using Org.Json;
//using 

namespace CWITC.Shared.DataStore.Firebase
{
    public abstract class BaseStore<T> : IBaseStore<T> where T : IBaseDataObject
    {
        TaskCompletionSource<bool> saveTask;
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

            var getData = new TaskCompletionSource<IEnumerable<T>>();

            var query = entityNode.OrderByPriority();
            query
                .AddListenerForSingleValueEvent(new ValueEventListenerCallback(getData));

            return getData.Task;
        }

        public virtual Task InitializeStore()
        {
            var rootNode = FirebaseDatabase.Instance.Reference;
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

            throw new NotImplementedException();
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

                throw new NotImplementedException();
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

                throw new NotImplementedException();
            }

            return false;
        }



        protected virtual DatabaseReference GetEntityNode(DatabaseReference rootNode)
        {
            return rootNode.Child(Identifier);
        }

        protected void ReloadEntityNode()
        {
            InitializeStore().Wait();
        }

        async Task<bool> SaveValues(object data)
        {
            saveTask = new TaskCompletionSource<bool>();
            //entityNode.SetValue(data, this);
            //entityNode.SetValue(data,
            //                    (NSError error, DatabaseReference reference) =>
            //{
            //    if (error != null) task.SetResult(false);

            //    task.SetResult(true);
            //});

            //return await task.Task;
            throw new NotImplementedException();
        }

        class ValueEventListenerCallback : Java.Lang.Object, IValueEventListener
        {
            TaskCompletionSource<bool> saveTask;
            TaskCompletionSource<IEnumerable<T>> getTask;

            public ValueEventListenerCallback(TaskCompletionSource<bool> saveTask)
            {
                this.saveTask = saveTask;    
            }

            public ValueEventListenerCallback(TaskCompletionSource<IEnumerable<T>> getTask)
            {
                this.getTask = getTask;
            }

            void IValueEventListener.OnCancelled(DatabaseError error)
            {
                throw new NotImplementedException();
            }

            void IValueEventListener.OnDataChange(DataSnapshot snapshot)
            {
                //snapshot.Value
                //throw new NotImplementedException();
            }
        }
    }
}