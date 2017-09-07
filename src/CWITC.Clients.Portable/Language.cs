using System;
namespace CWITC.Clients.Portable
{
    public static class Language
    {
        public static string Cancel = "Cancel";
        public static string More = "More";

        public static string VenueName = "Mid-State";
        public static string CallVenue => string.Format(CallLocationFormat, "Venu");
        public static string GetDirections = "Get Directions";
        public static string CallLocationFormat = "Call {0}";

        public static string ViewMenu = "View Menu";    
    }
}
