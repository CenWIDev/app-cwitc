using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
#if __IOS__
using Foundation;
#elif __ANDROID__
using Java.Lang;
using Java.Util;
#endif

[assembly: Dependency(typeof(FeedbackStore))]
namespace CWITC.Shared.DataStore
{
    public class FeedbackStore : BaseUserDataStore<Feedback>, IFeedbackStore
    {
        public override string Identifier => "session-feedback";

        public async Task<bool> LeftFeedback(Session session)
        {
            var userFeedback = await base.GetItemsAsync();

            var found = userFeedback?.FirstOrDefault(f => f.ContentfulId == session.Id);

            return found != null;
        }

		protected override IEnumerable<Feedback> ParseJsonResults(JToken json)
		{
			var dict = json.ToObject<Dictionary<string, Feedback>>();

			return dict.Values.ToList();
		}

#if __IOS__
		protected override NSDictionary GetSaveValue(IEnumerable<Feedback> existingItems)
		{
			var dict = GetSaveDictionary(existingItems);

			var result = NSDictionary
				.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());

			return result;
		}
#elif __ANDROID__
		protected override Java.Lang.Object GetSaveValue(IEnumerable<Feedback> existingItems)
		{
			var dict = GetSaveDictionary(existingItems);
			var map = new HashMap();

			foreach (var key in dict.Keys)
			{
				map.Put(new Java.Lang.String(key), new FeedbackJavaObj
				{
					ContentfulId = dict[key].ContentfulId,
					FeedbackText = dict[key].FeedbackText,
					SessionRating = dict[key].SessionRating
				});
			}

			return map;
		}

		class FeedbackJavaObj : Java.Lang.Object
		{
			public string ContentfulId { get; set; }

			public string FeedbackText { get; set; }

			public double SessionRating { get; set; }
		}
#endif

		Dictionary<string, Feedback> GetSaveDictionary(IEnumerable<Feedback> items)
		{
			Dictionary<string, Feedback> tosave = new Dictionary<string, Feedback>();
			foreach (var item in items)
			{
				tosave.Add(item.ContentfulId, item);
			}

			return tosave;
		}
	}
}
