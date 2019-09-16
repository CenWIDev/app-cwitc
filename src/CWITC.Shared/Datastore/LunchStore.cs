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
			return Task.FromResult(new LunchLocation
			{
				Name = entity.Name,
				Website = entity.SiteURL,
				// todo: the rest of these fields
			});
		}
	}
}
