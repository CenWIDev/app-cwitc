using System;
using Contentful.Core.Models;

namespace CWITC.Shared
{
	public class CategoryEntity
	{
		public SystemProperties Sys { get; set; }

		public string Name { get; set; }

		public string ShortName { get; set; }

		public string Color { get; set; }
	}
}
