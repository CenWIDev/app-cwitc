using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace CWITC.Clients.Portable
{
	public static class Config
	{
		static JObject Values { get; }

		static Config()
		{
			var thisAssembly = typeof(Config).Assembly;
			using (var stream = thisAssembly.GetManifestResourceStream("CWITC.Clients.Portable.Config.json"))
			{
				using (var reader = new StreamReader(stream))
				{
					var json = reader.ReadToEnd();

					Values = JObject.Parse(json);
				}
			}
		}

		public static string FacebookAppId => GetConfigValue();

		public static string GithubClientId => GetConfigValue();
		public static string GithubClientSecret => GetConfigValue();

		public static string TwitterClientId => GetConfigValue();
		public static string TwitterClientSecret => GetConfigValue();

		public static string ContentfulSpaceKey => GetConfigValue();
		public static string ContentfulDeliveryApiKey => GetConfigValue();

		static string GetConfigValue([CallerMemberName]string property = null) =>
			Values[property].ToString();
	}
}