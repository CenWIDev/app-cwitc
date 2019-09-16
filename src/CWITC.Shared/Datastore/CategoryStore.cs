using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore;
using Xamarin.Forms;

[assembly: Dependency(typeof(CategoryStore))]
namespace CWITC.Shared.DataStore
{
    public class CategoryStore : ContentfulDataStore<CategoryEntity, Category>, ICategoryStore
    {
        public override string Identifier => "sessionCategory";

		protected override Task<Category> Map(CategoryEntity entity)
		{
			return Task.FromResult(new Category
			{
				Id = entity?.Sys.Id,
				Name = entity?.Name,
				ShortName = entity?.ShortName,
				Color = entity?.Color
			});
		}
	}
}
