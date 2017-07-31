using System;
using System.Threading.Tasks;
using CWITC.DataObjects;

namespace CWITC.DataStore.Abstractions
{
    public interface IFeedbackStore : IBaseStore<Feedback>
    {
        Task<bool> LeftFeedback(Session session);
    }
}

