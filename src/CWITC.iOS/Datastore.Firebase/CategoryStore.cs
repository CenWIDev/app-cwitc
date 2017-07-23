using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.iOS.DataStore.Firebase;
using Xamarin.Forms;

[assembly: Dependency(typeof(CategoryStore))]
namespace CWITC.iOS.DataStore.Firebase
{
    public class CategoryStore : ReadonlyStore<Category>, ICategoryStore
    {
        public override string Identifier => "sessions";

        public override async Task<IEnumerable<Category>> GetItemsAsync(bool forceRefresh = false)
		{
			var sessionStore = DependencyService.Get<ISessionStore>();
			var sessions = await sessionStore.GetItemsAsync();

            var categories = sessions.Select(s => s.MainCategory).ToList();
            var categoryIds = categories.Select(x => x.Id).Distinct();

            return categoryIds.Select(id => categories.FirstOrDefault(x2 => x2.Id == id)).ToList();
		}

        public override async Task<Category> GetItemAsync(string id)
		{
			var speakers = await GetItemsAsync();
            var category = speakers.FirstOrDefault(s => s.Id == id);

			return category;
		}
    }
}
