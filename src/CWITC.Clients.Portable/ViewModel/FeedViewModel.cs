﻿using System;
using System.Windows.Input;
using System.Threading.Tasks;
using MvvmHelpers;
using System.Linq;
using Xamarin.Forms;
using FormsToolkit;
using System.Reflection;
using PCLStorage;
using Plugin.EmbeddedResource;
using Newtonsoft.Json;
using CWITC.DataObjects;
using System.Net.Http;
using System.Collections.Generic;
using CWITC.DataStore.Abstractions;
using CWITC.Clients.Portable.Services;

namespace CWITC.Clients.Portable
{
    public class FeedViewModel : ViewModelBase
    {

        public ObservableRangeCollection<Tweet> Tweets { get; } = new ObservableRangeCollection<Tweet>();
        public ObservableRangeCollection<Session> Sessions { get; } = new ObservableRangeCollection<Session>();
        public DateTime NextForceRefresh { get; set; }
        public FeedViewModel()
        {
            NextForceRefresh = DateTime.UtcNow.AddMinutes(45);

            MessagingService.Current.Subscribe(MessageKeys.TwitterAuthRefreshed, (msg) => LoadSocialCommand.Execute(null));
        }


        ICommand  refreshCommand;
        public ICommand RefreshCommand =>
            refreshCommand ?? (refreshCommand = new Command(async () => await ExecuteRefreshCommandAsync())); 

        async Task ExecuteRefreshCommandAsync()
        {
            try
            {
                NextForceRefresh = DateTime.UtcNow.AddMinutes(45);
                IsBusy = true;
                var tasks = new Task[]
                    {
                        ExecuteLoadNotificationsCommandAsync(),
                        ExecuteLoadSocialCommandAsync(),
                        ExecuteLoadSessionsCommandAsync()
                    };

                await Task.WhenAll(tasks);
            }
            catch(Exception ex)
            {
                ex.Data["method"] = "ExecuteRefreshCommandAsync";
                Logger.Report(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        Notification notification;
        public Notification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, value); }
        }

        bool loadingNotifications;
        public bool LoadingNotifications
        {
            get { return loadingNotifications; }
            set { SetProperty(ref loadingNotifications, value); }
        }

        bool noNotifications;
        public bool NoNotifications
		{
            get { return noNotifications; }
			set { SetProperty(ref noNotifications, value); }
		}

        ICommand  loadNotificationsCommand;
        public ICommand LoadNotificationsCommand =>
            loadNotificationsCommand ?? (loadNotificationsCommand = new Command(async () => await ExecuteLoadNotificationsCommandAsync())); 

        async Task ExecuteLoadNotificationsCommandAsync()
        {
            if (LoadingNotifications)
                return;
            LoadingNotifications = true;
            #if DEBUG
            await Task.Delay(1000);
            #endif

            try
            {
                Notification = await StoreManager.NotificationStore.GetLatestNotification();
            }
            catch(Exception ex)
            {
                ex.Data["method"] = "ExecuteLoadNotificationsCommandAsync";
                Logger.Report(ex);
                Notification = new Notification
                    {
                        Date = DateTime.UtcNow,
                        Text = "Welcome to CWITC!"
                    };   
            }
            finally
            {
                LoadingNotifications = false;
                NoNotifications = Notification == null;
            }
        }

        bool loadingSessions;
        public bool LoadingSessions
        {
            get { return loadingSessions; }
            set { SetProperty(ref loadingSessions, value); }
        }


        ICommand  loadSessionsCommand;
        public ICommand LoadSessionsCommand =>
            loadSessionsCommand ?? (loadSessionsCommand = new Command(async () => await ExecuteLoadSessionsCommandAsync())); 

        async Task ExecuteLoadSessionsCommandAsync()
        {
            if (LoadingSessions)
                return;
            
            LoadingSessions = true;

            try
            {
                NoSessions = false;
                Sessions.Clear();
                OnPropertyChanged("Sessions");
                #if DEBUG
                await Task.Delay(1000);
                #endif
                var sessions = await StoreManager.SessionStore.GetNextSessions();
                if(sessions != null)
                    Sessions.AddRange(sessions);

                NoSessions = Sessions.Count == 0;
            }
            catch(Exception ex)
            {
                ex.Data["method"] = "ExecuteLoadSessionsCommandAsync";
                Logger.Report(ex);
                NoSessions = true;
            }
            finally
            {
                LoadingSessions = false;
            }
            
        }

        bool noSessions;
        public bool NoSessions
        {
            get { return noSessions; }
            set { SetProperty(ref noSessions, value); }
        }

        Session selectedSession;
        public Session SelectedSession
        {
            get { return selectedSession; }
            set
            {
                selectedSession = value;
                OnPropertyChanged();
                if (selectedSession == null)
                    return;

                MessagingService.Current.SendMessage(MessageKeys.NavigateToSession, selectedSession);

                SelectedSession = null;
            }
        }

        bool loadingSocial;
        public bool LoadingSocial
        {
            get { return loadingSocial; }
            set { SetProperty(ref loadingSocial, value); }
        }

        bool noSocial;
        public bool NoSocial
        {
            get { return noSocial; }
            set { SetProperty(ref noSocial, value);}
        }

        ICommand  loadSocialCommand;
        public ICommand LoadSocialCommand =>
            loadSocialCommand ?? (loadSocialCommand = new Command(async () => await ExecuteLoadSocialCommandAsync())); 

        async Task ExecuteLoadSocialCommandAsync()
        {
            if (LoadingSocial)
                return;

            LoadingSocial = true;
            try
            {
                SocialError = false;
                Tweets.Clear();

                var tweetService = DependencyService.Get<ITweetsService>();

                Tweets.ReplaceRange(await tweetService.GetTweets());
                // todo: implement this
            }
            catch (Exception ex)
            {
                SocialError = true;
                ex.Data["method"] = "ExecuteLoadSocialCommandAsync";
                Logger.Report(ex);
            }
            finally
            {
                LoadingSocial = false;
                NoSocial = Tweets.Count == 0;
            }

        }

        bool socialError;
        public bool SocialError
        {
            get { return socialError; }
            set { SetProperty(ref socialError, value); }
        }

        Tweet selectedTweet;
        public Tweet SelectedTweet
        {
            get { return selectedTweet; }
            set
            {
                selectedTweet = value;
                OnPropertyChanged();
                if (selectedTweet == null)
                    return;

                LaunchBrowserCommand.Execute(selectedTweet.Url);

                SelectedTweet = null;
            }
        }

        ICommand  favoriteCommand;
        public ICommand FavoriteCommand =>
        favoriteCommand ?? (favoriteCommand = new Command<Session>((s) => ExecuteFavoriteCommand(s))); 

        void ExecuteFavoriteCommand(Session session)
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.Question, new MessagingServiceQuestion
                {
                    Negative = "Cancel",
                    Positive = "Unfavorite",
                    Question = "Are you sure you want to remove this session from your favorites?",
                    Title = "Unfavorite Session",
                    OnCompleted = (async (result) =>
                        {
                            if(!result)
                                return;

                            var toggled = await FavoriteService.ToggleFavorite(session);
                            if(toggled)
                                await ExecuteLoadSessionsCommandAsync();
                        })
                });
            
        }
    }
}

