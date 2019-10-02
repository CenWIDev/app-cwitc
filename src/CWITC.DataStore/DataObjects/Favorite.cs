using System.Collections.Generic;
using Newtonsoft.Json;

namespace CWITC.DataObjects
{
	/// <summary>
	/// This is per user
	/// </summary>
	public class Favorite : BaseDataObject
	{
		[JsonIgnore]
		public override string Id
		{
			get => this.ContentfulId;
			set => this.ContentfulId = value;
		}

		[JsonProperty("contentfulId")]
		public string ContentfulId { get; set; }
	}
}