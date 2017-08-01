using System;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using Firebase.Database;
using FormsToolkit;

namespace CWITC.Shared.DataStore.Firebase
{
    public abstract class BaseUserDataStore<T> : BaseStore<T>
        where T : IBaseDataObject
    {
        public override System.Threading.Tasks.Task InitializeStore()
        {
            MessagingService.Current.Subscribe(MessageKeys.LoggedIn, (m) =>
            {
                ReloadEntityNode();
            });
            return base.InitializeStore();
        }

        protected override DatabaseReference GetEntityNode(DatabaseReference rootNode)
        {
            // nodes will be keyed by the user
            return rootNode.Child(Identifier).Child(Settings.Current.UserId);
        }
    }
}