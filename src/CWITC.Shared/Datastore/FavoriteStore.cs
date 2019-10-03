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

[assembly:Dependency(typeof(FavoriteStore))]
namespace CWITC.Shared.DataStore
{
    public class FavoriteStore : BaseUserDataStore<Favorite>, IFavoriteStore
    {
        public override string Identifier => "favorited-sessions";

        public async Task<bool> IsFavorite(string sessionId)
        {
            var items = (await GetItemsAsync()) ?? new List<Favorite>();
            return items.Any(x => x.ContentfulId == sessionId);
        }

		protected override IEnumerable<Favorite> ParseJsonResults(JToken json)
		{
			var dict = json.ToObject<Dictionary<string, Favorite>>();

			return dict.Values.ToList();
		}

#if __IOS__
		protected override NSDictionary GetSaveValue(IEnumerable<Favorite> existingItems)
		{
			var dict = GetSaveDictionary(existingItems);
			var json = JsonConvert.SerializeObject(dict);

			var data = NSData.FromString(json);
			var result =NSJsonSerialization.Deserialize(data, NSJsonReadingOptions.AllowFragments, out NSError readError) as NSDictionary;

			return result;
		}

#elif __ANDROID__
		protected override Java.Lang.Object GetSaveValue(IEnumerable<Favorite> existingItems)
		{
			var dict = GetSaveDictionary(existingItems);

			var map = new HashMap();

			foreach (var key in dict.Keys)
			{
				var item = new HashMap();
				item.Put(new Java.Lang.String("contentfulId"), new Java.Lang.String(dict[key].ContentfulId));

				map.Put(key, item);
			}

			return map;
		}		
#endif

		Dictionary<string, Favorite> GetSaveDictionary(IEnumerable<Favorite> items)
		{
			Dictionary<string, Favorite> tosave = new Dictionary<string, Favorite>();
			foreach (var item in items)
			{
				tosave.Add(item.ContentfulId, item);
			}

			return tosave;
		}

#if __ANDROID__
		class FavoriteJavaObj : Java.Lang.Object
#elif __IOS__
		class FavoriteNSObject : NSObject
#endif
		{
			public string ContentfulId { get; set; }
		}
	}
}