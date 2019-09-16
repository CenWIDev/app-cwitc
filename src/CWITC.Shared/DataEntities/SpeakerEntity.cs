using System;
using Contentful.Core.Models;

namespace CWITC.Shared
{
	public class SpeakerEntity
	{
		public SystemProperties Sys { get; set; }

		public string Name { get; set; }

		public string PositionName { get; set; }

		public string Biography { get; set; }

		public string CompanyName { get; set; }

		public string CompanyWebsiteURL { get; set; }
	}
}
