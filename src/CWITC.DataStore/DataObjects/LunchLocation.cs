using System;
namespace CWITC.DataObjects
{
    public class LunchLocation : BaseDataObject
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string ImageUri { get; set; }

        public string Website { get; set; }
    }
}
