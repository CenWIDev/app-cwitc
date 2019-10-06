using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace CWITC.Shared
{
	public class SessionEntity
	{
		public SystemProperties Sys { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("description")]
		public Document Description { get; set; }

		[JsonProperty("sessionType")]
		public string SessionType { get; set; }

		[JsonProperty("startTime")]
		public DateTime StartTime { get; set; }

		[JsonProperty("endTime")]
		public DateTime EndTime { get; set; }

		[JsonProperty("room")]
		public string Room { get; set; }

		[JsonProperty("category")]
		public CategoryEntity Category { get; set; }

		[JsonProperty("speakers")]
		public IEnumerable<SpeakerEntity> Speakers { get; set; }
	}
}
