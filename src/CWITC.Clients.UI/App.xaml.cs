using System;
using System.Collections.Generic;
using FormsToolkit;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;
using CWITC.Clients.Portable;

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
			// The root page of your application
			switch (Device.RuntimePlatform)
			{
				case "Android":
					MainPage = new RootPageAndroid();
					break;
				case "iOS":
					MainPage = new EvolveNavigationPage(new RootPageiOS());
					break;
				default:
					throw new NotImplementedException();
			}
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



			MessagingService.Current.Subscribe(MessageKeys.NavigateLogin, async m =>
				{
					if (Device.RuntimePlatform == "Android")
					{
						((RootPageAndroid)MainPage).IsPresented = false;
					}

					var auth = DependencyService.Get<IAuthClient>();

					bool shouldSignIn = true;
					if (Settings.Current.FirstRun)
					{
						shouldSignIn = await this.MainPage.DisplayAlert("Sign In?", "Sign in to save and sync favorites and leave session feedback.", "Ok", "Maybe Later");
					}

					if (shouldSignIn)
					{
						await auth.LoginAsync();
					}
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

	}
}

