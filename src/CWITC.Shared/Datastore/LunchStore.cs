using System;
using System.Threading.Tasks;
using Contentful.Core.Search;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared;
using CWITC.Shared.DataStore;
using Xamarin.Forms;

[assembly: Dependency(typeof(LunchStore))]
namespace CWITC.Shared
{
    public partial class LunchStore : ContentfulDataStore<PartnerEntity, LunchLocation>, ILunchStore
    {
        public override string Identifier => "partner";

		protected override QueryBuilder<PartnerEntity> GetYearFilter()
		{
			return new QueryBuilder<PartnerEntity>()
				.FieldEquals(p => p.PartnerType, "Lunch Venue");
		}

		protected override Task<LunchLocation> Map(PartnerEntity entity)
		{
			var loc = new LunchLocation
			{
				Name = entity.Name,
				Website = entity.SiteURL,
				Address = entity.Address

				// todo: the rest of these fields
			};
			if (entity.Logo != null)
				loc.ImageUri = $"https:{entity.Logo.File.Url}";

			return Task.FromResult(loc);
		}
	}
}
