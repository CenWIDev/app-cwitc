using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using CWITC.Clients.Portable;
using System.Diagnostics;
[assembly: Dependency(typeof(EvolveLogger))]
namespace CWITC.Clients.Portable
{
	public class EvolveLogger : ILogger
	{
		bool enableHockeyApp = false;

		public virtual void TrackPage(string page, string id = null)
		{
			Debug.WriteLine("Evolve Logger: TrackPage: " + page.ToString() + " Id: " + id ?? string.Empty);

			if (!enableHockeyApp)
				return;

			Microsoft.AppCenter.Analytics.Analytics.TrackEvent($"{page}Page");
		}


		public virtual void Track(string trackIdentifier)
		{
			Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier);

			if (!enableHockeyApp)
				return;

			Microsoft.AppCenter.Analytics.Analytics.TrackEvent(trackIdentifier);
		}

		public virtual void Track(string trackIdentifier, string key, string value)
		{
			Debug.WriteLine("Evolve Logger: Track: " + trackIdentifier + " key: " + key + " value: " + value);

			if (!enableHockeyApp)
				return;

			trackIdentifier = $"{trackIdentifier}-{key}-{@value}";

			Microsoft.AppCenter.Analytics.Analytics.TrackEvent(trackIdentifier);
		}

		public virtual void Report(Exception exception = null, Severity warningLevel = Severity.Warning)
		{
			Debug.WriteLine("Evolve Logger: Report: " + exception);
			Microsoft.AppCenter.Crashes.Crashes.TrackError(exception);
		}
		public virtual void Report(Exception exception, IDictionary extraData, Severity warningLevel = Severity.Warning)
		{
			// todo: add extra data
			Microsoft.AppCenter.Crashes.Crashes.TrackError(exception);
		}
		public virtual void Report(Exception exception, string key, string value, Severity warningLevel = Severity.Warning)
		{
			Microsoft.AppCenter.Crashes.Crashes.TrackError(exception);
		}
	}
}