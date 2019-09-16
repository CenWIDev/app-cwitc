using System;
using System.Threading.Tasks;
using Contentful.Core.Search;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.Shared.DataStore;
using Xamarin.Forms;

[assembly: Dependency(typeof(SponsorStore))]
namespace CWITC.Shared.DataStore
{
    public class SponsorStore : ContentfulDataStore<PartnerEntity, Sponsor>, ISponsorStore
    {
        public override string Identifier => "partner";

		protected override QueryBuilder<PartnerEntity> GetYearFilter()
		{
			return new QueryBuilder<PartnerEntity>()
				.FieldIncludes(p => p.PartnerType, new string[] { "Organizer", "Sponsor" })
				.FieldDoesNotEqual("sys.id", "2MuAW93azJnXPxY62jfDWz");
		}

		protected override Task<Sponsor> Map(PartnerEntity entity)
		{
			var s = new Sponsor
			{
				Name = entity.Name,
				//Description = entity.
				ImageUrl = $"https:{entity.Logo.File.Url}",
				WebsiteUrl = entity.SiteURL,
				SponsorLevel = new SponsorLevel
				{
					Name = entity.SponsorshipLevel
				}
			};

			if (entity.PartnerType == "Organizer")
			{
				s.Rank = 1;
			}
			else
			{
				switch (entity.SponsorshipLevel)
				{
					case "Gold":
						s.Rank = 2;
						break;
					case "Silver":
						s.Rank = 3;
						break;
					case "Bronze":
						s.Rank = 4; break;
					case "Other":
						s.Rank = 5;
						break;
				}
			}

			return Task.FromResult(s);
		}
	}
}
