// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using CWITC.Clients.Portable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CWITC.Clients.Portable
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public class Settings : INotifyPropertyChanged
    {
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        static Settings settings;

        /// <summary>
        /// Gets or sets the current settings. This should always be used
        /// </summary>
        /// <value>The current.</value>
        public static Settings Current
        {
            get { return settings ?? (settings = new Settings()); }
        }

        const string AuthTypeKey = "auth_type_key";
        readonly string AuthTypeDefault = string.Empty;
        public string AuthType
        {
            get { return AppSettings.GetValueOrDefault<string>(AuthTypeKey, AuthTypeDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(AuthTypeKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        const string TwitterApiKeyKey = "twitter_api_key";
        readonly string TwitterApiKeyDefault = string.Empty;
        public string TwitterApiKey
        {
            get { return AppSettings.GetValueOrDefault<string>(TwitterApiKeyKey, TwitterApiKeyDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(TwitterApiKeyKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        const string TwitterApiSecretKey = "twitter_api_secret";
        readonly string TwitterApiSecretDefault = string.Empty;
        public string TwitterApiSecret
        {
            get { return AppSettings.GetValueOrDefault<string>(TwitterApiSecretKey, TwitterApiSecretDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(TwitterApiSecretKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        const string GcmTokenKey = "gcm_token";
        readonly string GcmTokenDefault = string.Empty;
        public string GcmToken
        {
            get { return AppSettings.GetValueOrDefault<string>(GcmTokenKey, GcmTokenDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(GcmTokenKey, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        const string LastFavoriteTimeKey = "last_favorite_time";

        public DateTime LastFavoriteTime
        {
            get { return AppSettings.GetValueOrDefault<DateTime>(LastFavoriteTimeKey, DateTime.UtcNow); }
            set
            {
                AppSettings.AddOrUpdateValue<DateTime>(LastFavoriteTimeKey, value);
            }
        }

        const string FirstRunKey = "first_run";
        static readonly bool FirstRunDefault = true;

        /// <summary>
        /// Gets or sets a value indicating whether the user wants to see favorites only.
        /// </summary>
        /// <value><c>true</c> if favorites only; otherwise, <c>false</c>.</value>
        public bool FirstRun
        {
            get { return AppSettings.GetValueOrDefault<bool>(FirstRunKey, FirstRunDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(FirstRunKey, value))
                    OnPropertyChanged();
            }
        }

        const string GooglePlayCheckedKey = "play_checked";
        static readonly bool GooglePlayCheckedDefault = false;

        public bool GooglePlayChecked
        {
            get { return AppSettings.GetValueOrDefault<bool>(GooglePlayCheckedKey, GooglePlayCheckedDefault); }
            set
            {
                AppSettings.AddOrUpdateValue<bool>(GooglePlayCheckedKey, value);
            }
        }

        const string AttemptedPushKey = "attempted_push";
        static readonly bool AttemptedPushDefault = false;

        public bool AttemptedPush
        {
            get { return AppSettings.GetValueOrDefault<bool>(AttemptedPushKey, AttemptedPushDefault); }
            set
            {
                AppSettings.AddOrUpdateValue<bool>(AttemptedPushKey, value);
            }
        }


        const string PushRegisteredKey = "push_registered";
        static readonly bool PushRegisteredDefault = false;

        public bool PushRegistered
        {
            get { return AppSettings.GetValueOrDefault<bool>(PushRegisteredKey, PushRegisteredDefault); }
            set
            {
                AppSettings.AddOrUpdateValue<bool>(PushRegisteredKey, value);
            }
        }

        const string FavoriteModeKey = "favorites_only";
        static readonly bool FavoriteModeDefault = false;

        /// <summary>
        /// Gets or sets a value indicating whether the user wants to see favorites only.
        /// </summary>
        /// <value><c>true</c> if favorites only; otherwise, <c>false</c>.</value>
        public bool FavoritesOnly
        {
            get { return AppSettings.GetValueOrDefault<bool>(FavoriteModeKey, FavoriteModeDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(FavoriteModeKey, value))
                    OnPropertyChanged();
            }
        }

        const string ShowAllCategoriesKey = "all_categories";
        static readonly bool ShowAllCategoriesDefault = true;

        /// <summary>
        /// Gets or sets a value indicating whether the user wants to show all categories.
        /// </summary>
        /// <value><c>true</c> if show all categories; otherwise, <c>false</c>.</value>
        public bool ShowAllCategories
        {
            get { return AppSettings.GetValueOrDefault<bool>(ShowAllCategoriesKey, ShowAllCategoriesDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(ShowAllCategoriesKey, value))
                    OnPropertyChanged();
            }
        }

        const string ShowPastSessionsKey = "show_past_sessions";
        static readonly bool ShowPastSessionsDefault = false;
        public static readonly DateTime EndOfEvolve = new DateTime(2016, 4, 29, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets or sets a value indicating whether the user wants show past sessions.
        /// </summary>
        /// <value><c>true</c> if show past sessions; otherwise, <c>false</c>.</value>
        public bool ShowPastSessions
        {
            get
            {
                //if end of evolve
                if (DateTime.UtcNow > EndOfEvolve)
                    return true;

                return AppSettings.GetValueOrDefault<bool>(ShowPastSessionsKey, ShowPastSessionsDefault);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(ShowPastSessionsKey, value))
                    OnPropertyChanged();
            }
        }

        const string FilteredCategoriesKey = "filtered_categories";
        static readonly string FilteredCategoriesDefault = string.Empty;


        public string FilteredCategories
        {
            get { return AppSettings.GetValueOrDefault<string>(FilteredCategoriesKey, FilteredCategoriesDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue<string>(FilteredCategoriesKey, value))
                    OnPropertyChanged();
            }
        }


        const string EmailKey = "email_key";
        readonly string EmailDefault = string.Empty;
        public string Email
        {
            get { return AppSettings.GetValueOrDefault<string>(EmailKey, EmailDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(EmailKey, value))
                {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UserAvatar));
                }
            }
        }

        const string DatabaseIdKey = "azure_database";
        static readonly int DatabaseIdDefault = 0;

        public static int DatabaseId
        {
            get { return AppSettings.GetValueOrDefault<int>(DatabaseIdKey, DatabaseIdDefault); }
            set
            {
                AppSettings.AddOrUpdateValue<int>(DatabaseIdKey, value);
            }
        }

        public static int UpdateDatabaseId()
        {
            return DatabaseId++;
        }

        const string UserIdKey = "userid_key";
        readonly string UserIdDefault = string.Empty;
        public string UserId
        {
            get { return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(UserIdKey, value))
                {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UserId));
                }
            }
        }

        const string FirstNameKey = "firstname_key";
        readonly string FirstNameDefault = string.Empty;
        public string FirstName
        {
            get { return AppSettings.GetValueOrDefault<string>(FirstNameKey, FirstNameDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(FirstNameKey, value))
                {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UserDisplayName));
                }
            }
        }

        const string LastNameKey = "lastname_key";
        readonly string LastNameDefault = string.Empty;
        public string LastName
        {
            get { return AppSettings.GetValueOrDefault<string>(LastNameKey, LastNameDefault); }
            set
            {
                if (AppSettings.AddOrUpdateValue(LastNameKey, value))
                {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UserDisplayName));
                }
            }
        }


        const string NeedsSyncKey = "needs_sync";
        const bool NeedsSyncDefault = true;
        public bool NeedsSync
        {
            get { return AppSettings.GetValueOrDefault<bool>(NeedsSyncKey, NeedsSyncDefault) || LastSync < DateTime.Now.AddDays(-1); }
            set { AppSettings.AddOrUpdateValue<bool>(NeedsSyncKey, value); }

        }

        const string LoginAttemptsKey = "login_attempts";
        const int LoginAttemptsDefault = 0;
        public int LoginAttempts
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(LoginAttemptsKey, LoginAttemptsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<int>(LoginAttemptsKey, value);
            }
        }

        const string HasSyncedDataKey = "has_synced";
        const bool HasSyncedDataDefault = false;
        public bool HasSyncedData
        {
            get { return AppSettings.GetValueOrDefault<bool>(HasSyncedDataKey, HasSyncedDataDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(HasSyncedDataKey, value); }

        }

        const string LastSyncKey = "last_sync";
        static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);
        public DateTime LastSync
        {
            get
            {
                return AppSettings.GetValueOrDefault<DateTime>(LastSyncKey, LastSyncDefault);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<DateTime>(LastSyncKey, value))
                    OnPropertyChanged();
            }
        }

        const string GrouveEventCodeKey = "grouve_event_code";
        static readonly string GrouveEventCodeDefault = ApiKeys.GrouveEventCode;
        public string GrouveEventCode
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(GrouveEventCodeKey, GrouveEventCodeDefault);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<string>(GrouveEventCodeKey, value))
                    OnPropertyChanged();
            }
        }

        bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected == value)
                    return;
                isConnected = value;
                OnPropertyChanged();
            }
        }

        #region Helpers


        public string UserDisplayName => IsLoggedIn ? $"{FirstName} {LastName}" : "Sign In";

        public string UserAvatar => IsLoggedIn ? Gravatar.GetURL(Email) : "profile_generic.png";

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Email);

        public bool HasFilters => (ShowPastSessions || FavoritesOnly || (!string.IsNullOrWhiteSpace(FilteredCategories) && !ShowAllCategories));

        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}