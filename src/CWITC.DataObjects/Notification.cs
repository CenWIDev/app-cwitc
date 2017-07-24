using System;

namespace CWITC.DataObjects
{
    public class Notification : BaseDataObject
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool IsVisible { get; set; }
    }
}

