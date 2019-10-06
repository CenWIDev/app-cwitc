using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace CWITC.Shared
{
	public class PartnerEntity
	{
		public string Name { get; set; }

		public string SiteURL { get; set; }

		public Asset Logo { get; set; }

		public string PartnerType { get; set; }

		public string SponsorshipLevel { get; set; }

		public string Address { get; set; }
	}
}
