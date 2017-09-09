

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
        Events,
        Gallery,
        Sponsors,
        Venue,
        FloorMap,
        Settings,
        Session,
        Speaker,
        Sponsor,
        Login,
        Event,
        Notification,
        TweetImage,
        Filter,
        Information,
        Tweet,
        LunchLocations,
        LunchLocation
    }
}


