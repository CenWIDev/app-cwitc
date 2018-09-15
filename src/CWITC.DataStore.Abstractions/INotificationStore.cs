using System;
using CWITC.DataObjects;
using System.Threading.Tasks;

namespace CWITC.DataStore.Abstractions
{
	public interface INotificationStore : IReadonlyStore<Notification>
    {
        Task<Notification> GetLatestNotification();
    }
}

