﻿

namespace CWITC.Clients.Portable
{
    public class DeepLinkPage
    {
        public AppPage Page { get; set; }
        public string Id { get; set;}
    }
    public enum AppPage
    {
        Feed,
        Sessions,
        Schedule,
        Sponsors,
        Venue,
        FloorMap,
        Settings,
        Session,
        Speaker,
        Sponsor,
        Login,
        Event,
        TweetImage,
        Filter,
        Information,
        Tweet,
        LunchLocations,
        LunchLocation
    }
}


