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
using Org.Json;
using JsonHelper = CWITC.Droid.JavaJsonHelperExtensions;

namespace CWITC.Shared.DataStore.Firebase
{
    public abstract partial class BaseStore<T> : IBaseStore<T> where T : IBaseDataObject
    {
        TaskCompletionSource<bool> saveTask;
        DatabaseReference entityNode;

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

        protected virtual DatabaseReference GetEntityNode(DatabaseReference rootNode)
        {
			return rootNode.Child("2018").Child(Identifier);
        }

        Task<bool> SaveValues(ArrayList data)
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

        ArrayList GetArray(IEnumerable<T> existingItems)
        {
            var arrayList = new ArrayList();

            string jsonString = JsonConvert.SerializeObject(existingItems);
            var jsonArray = new JSONArray(jsonString);

            return JsonHelper.ToJavaList(jsonArray);
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
                this.getTask.TrySetCanceled();
            }

            void IValueEventListener.OnDataChange(DataSnapshot snapshot)
            {
                var values = (snapshot.Value as ArrayList)?.ToArray();

                if (values != null)
                {
                    List<T> items = new List<T>();

                    foreach (var value in values)
                    {
                        var data = new GoogleGson.Gson().ToJson(value);

                        var item = JsonConvert.DeserializeObject<T>(data);
                        items.Add(item);
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