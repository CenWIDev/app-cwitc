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
using Newtonsoft.Json;
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
			var json = JsonConvert.SerializeObject(dict);

			var data = NSData.FromString(json);
			var result = NSJsonSerialization.Deserialize(data, NSJsonReadingOptions.AllowFragments, out NSError readError) as NSDictionary;

			return result;
		}

#elif __ANDROID__
		protected override Java.Lang.Object GetSaveValue(IEnumerable<Feedback> existingItems)
		{
			var dict = GetSaveDictionary(existingItems);

			var map = new HashMap();

			foreach (var key in dict.Keys)
			{
				var item = new HashMap();
				item.Put(new Java.Lang.String("contentfulId"), new Java.Lang.String(dict[key].ContentfulId));
				item.Put(new Java.Lang.String("feedbackText"), new Java.Lang.String(dict[key].FeedbackText));
				item.Put(new Java.Lang.String("sessionRating"), new Java.Lang.Double(dict[key].SessionRating));

				map.Put(key, item);
			}

			return map;
		}		
#endif

		Dictionary<string, Feedback> GetSaveDictionary(IEnumerable<Feedback> items)
		{
			var tosave = new Dictionary<string, Feedback>();
			foreach (var item in items)
			{
				tosave.Add(item.ContentfulId, item);
			}

			return tosave;
		}

#if __ANDROID__
		class FeedbackJavaObj : Java.Lang.Object
#elif __IOS__
		class FeedbackNSObject : NSObject
#endif
		{
			public string ContentfulId { get; set; }

			public string FeedbackText { get; set; }

			public double SessionRating { get; set; }
		}
	}
}
