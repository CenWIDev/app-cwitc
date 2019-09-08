using System;
using System.Collections.Generic;
using FormsToolkit;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;
using CWITC.Clients.Portable;
using Plugin.Settings;

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace CWITC.Clients.UI
{
	public partial class App : Application
	{
		public static App current;
		public App()
		{
			current = this;
			InitializeComponent();
			ViewModelBase.Init();

			this.ResetMainPage();
		}

		static ILogger logger;
		public static ILogger Logger => logger ?? (logger = DependencyService.Get<ILogger>());

		protected override void OnStart()
		{
			OnResume();
		}

		public void SecondOnResume()
		{
			OnResume();
		}

		bool registered;
		bool firstRun = true;
		protected override void OnResume()
		{
			if (registered)
				return;
			registered = true;
			// Handle when your app resumes
			Settings.Current.IsConnected = CrossConnectivity.Current.IsConnected;
			CrossConnectivity.Current.ConnectivityChanged += ConnectivityChanged;

			// Handle when your app starts
			MessagingService.Current.Subscribe<MessagingServiceAlert>(MessageKeys.Message, async (m, info) =>
				{
					var task = Application.Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);

					if (task == null)
						return;

					await task;
					info?.OnCompleted?.Invoke();
				});


			MessagingService.Current.Subscribe<MessagingServiceQuestion>(MessageKeys.Question, async (m, q) =>
				{
					var task = Application.Current?.MainPage?.DisplayAlert(q.Title, q.Question, q.Positive, q.Negative);
					if (task == null)
						return;
					var result = await task;
					q?.OnCompleted?.Invoke(result);
				});

			MessagingService.Current.Subscribe<MessagingServiceChoice>(MessageKeys.Choice, async (m, q) =>
				{
					var task = Application.Current?.MainPage?.DisplayActionSheet(q.Title, q.Cancel, q.Destruction, q.Items);
					if (task == null)
						return;
					var result = await task;
					q?.OnCompleted?.Invoke(result);
				});

			MessagingService.Current.Subscribe(MessageKeys.LoggedIn, (m) =>
			{
				this.ResetMainPage();
			});

			MessagingService.Current.Subscribe(MessageKeys.LoggedOut, (m) =>
			{
				this.ResetMainPage();
			});

			try
			{
				if (firstRun || Device.RuntimePlatform != "iOS")
					return;

				var mainNav = MainPage as NavigationPage;
				if (mainNav == null)
					return;

				var rootPage = mainNav.CurrentPage as RootPageiOS;
				if (rootPage == null)
					return;

				var rootNav = rootPage.CurrentPage as NavigationPage;
				if (rootNav == null)
					return;


				var about = rootNav.CurrentPage as AboutPage;
				if (about != null)
				{
					about.OnResume();
					return;
				}
				var sessions = rootNav.CurrentPage as SessionsPage;
				if (sessions != null)
				{
					sessions.OnResume();
					return;
				}
				var feed = rootNav.CurrentPage as FeedPage;
				if (feed != null)
				{
					feed.OnResume();
					return;
				}
			}
			catch
			{
			}
			finally
			{
				firstRun = false;
			}
		}

		protected override void OnSleep()
		{
			if (!registered)
				return;

			registered = false;
			MessagingService.Current.Unsubscribe(MessageKeys.NavigateLogin);
			MessagingService.Current.Unsubscribe<MessagingServiceQuestion>(MessageKeys.Question);
			MessagingService.Current.Unsubscribe<MessagingServiceAlert>(MessageKeys.Message);
			MessagingService.Current.Unsubscribe<MessagingServiceChoice>(MessageKeys.Choice);
			MessagingService.Current.Unsubscribe<MessagingServiceChoice>(MessageKeys.LoggedIn);
			MessagingService.Current.Unsubscribe<MessagingServiceChoice>(MessageKeys.LoggedOut);

			// Handle when your app sleeps
			CrossConnectivity.Current.ConnectivityChanged -= ConnectivityChanged;
		}

		protected async void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
			//save current state and then set it
			var connected = Settings.Current.IsConnected;
			Settings.Current.IsConnected = e.IsConnected;
			if (connected && !e.IsConnected)
			{
				//we went offline, should alert the user and also update ui (done via settings)
				var task = Application.Current?.MainPage?.DisplayAlert("Offline", "Uh Oh, It looks like you have gone offline. Please check your internet connection to get the latest data and enable syncing data.", "OK");
				if (task != null)
					await task;
			}
		}

		private void ResetMainPage()
		{
			switch (Device.RuntimePlatform)
			{
				case "Android":
					MainPage = Settings.Current.IsLoggedIn ? (Page)new RootPageAndroid() : (Page)new LoginPage();
					break;
				case "iOS":
					MainPage = new EvolveNavigationPage(Settings.Current.IsLoggedIn ? (Page)new RootPageiOS() : new LoginPage());
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}

