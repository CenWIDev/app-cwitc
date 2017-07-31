using System;
using System.Linq;
using System.Threading.Tasks;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.iOS.DataStore.Firebase;
using Xamarin.Forms;

[assembly: Dependency(typeof(FeedbackStore))]
namespace CWITC.iOS.DataStore.Firebase
{
    public class FeedbackStore : BaseUserDataStore<Feedback>, IFeedbackStore
    {
        public override string Identifier => "user_session_feedback";

        public async Task<bool> LeftFeedback(Session session)
        {
            var userFeedback = await base.GetItemsAsync();

            var found = userFeedback?.FirstOrDefault(f => f.SessionId == session.Id);

            return found != null;
        }
    }
}
