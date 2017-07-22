using System;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using Firebase.Database;

namespace CWITC.iOS.DataStore.Firebase
{
    public abstract class BaseUserDataStore<T> : BaseStore<T> 
        where T : IBaseDataObject
    {
        protected override DatabaseReference GetEntityNode(DatabaseReference rootNode)
		{
            // nodes will be keyed by the user
			return rootNode.GetChild(Identifier).GetChild(Settings.Current.UserId);
		}
    }
}