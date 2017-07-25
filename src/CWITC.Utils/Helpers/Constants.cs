using System;

namespace CWITC.Clients.Portable
{
   
    public static class ApiKeys
    {
		// todo: change these values
		public const string HockeyAppiOS = "HockeyAppiOS";
        public const string HockeyAppAndroid = "HockeyAppAndroid";
        public const string HockeyAppUWP = "HockeyAppUWP";

		// todo: change these values
		public const string AzureServiceBusName = "AzureServiceBusName";
        public const string AzureServiceBusUrl = "AzureServiceBusUrl";
        public const string AzureKey ="AzureKey";


        // todo: change these values
        public const string GoogleSenderId ="xamarinawareness";
        public const string AzureHubName = "EvolveTest";
        public const string AzureListenConneciton = "Endpoint=sb://evolvetest.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nC00PmSAZu5jv1v8qRGBvS6PwpiW9R5V7CrMhf0lWiA=";
    }
    public static class MessageKeys
    {
        public const string LoginCallback = "auth_login_callback";
        public const string LogoutCallback = "auth_logout_callback";
        public const string NavigateToEvent = "navigate_event";
        public const string NavigateToSession = "navigate_session";
        public const string NavigateToSpeaker = "navigate_speaker";
        public const string NavigateToSponsor = "navigate_sponsor";
        public const string NavigateToImage = "navigate_image";
        public const string NavigateLogin = "navigate_login";
        public const string Error = "error";
        public const string Connection = "connection";
        public const string LoggedIn = "loggedin";
        public const string Message = "message";
        public const string Question = "question";
        public const string Choice = "choice";
        public const string TwitterAuthRefreshed = "refresh_tweets";
    }
}

