using System;
using System.Windows.Input;
using System.Threading.Tasks;
using FormsToolkit;
using Plugin.Connectivity;
using Plugin.Share;
using Xamarin.Forms;
using MvvmHelpers;
using Humanizer;

namespace CWITC.Clients.Portable
{
	public class SettingsViewModel : ViewModelBase
	{
		public ObservableRangeCollection<MenuItem> AboutItems { get; } = new ObservableRangeCollection<MenuItem>();

		public SettingsViewModel()
		{
			AboutItems.AddRange(new[]
				{
					new MenuItem { Name = "Created by CENWIDEV with <3", Command=LaunchBrowserCommand, Parameter="http://cenwidev.org" },
					new MenuItem { Name = "Open source on GitHub!", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc"},
					new MenuItem { Name = "Terms of Use", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc/wiki/Terms-&-Conditions"},
					new MenuItem { Name = "Privacy Policy", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc/wiki/Privacy-Policy"},
					new MenuItem { Name = "Code of Conduct", Command=LaunchBrowserCommand, Parameter="https://cwitc.org/code-of-conduct"},
					new MenuItem { Name = "Open Source Notice", Command=LaunchBrowserCommand, Parameter="https://github.com/CenWIDev/app-cwitc/tree/master/oss-licenses"}
				});
		}

		public string LastSyncDisplay
		{
			get
			{
				if (!Settings.HasSyncedData)
					return "Never";

				return Settings.LastSync.Humanize();
			}
		}

		public string Copyright => $"Copyright {DateTime.Today.Year} - CENWIDEV.";
	}
}

