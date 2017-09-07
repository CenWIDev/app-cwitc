using System;
namespace CWITC.DataObjects
{
    public class LunchLocation : BaseDataObject
    {
        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PhoneNumber { get; set; }

        public string Category { get; set; }

        public string ImageUri { get; set; }

        public string Menu { get; set; }

        public LunchLocation()
        {
        }
    }
}
