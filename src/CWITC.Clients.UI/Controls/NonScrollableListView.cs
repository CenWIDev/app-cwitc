using System;
using Xamarin.Forms;

namespace CWITC.Clients.UI
{
    public class NonScrollableListView : ListView
    {
        public NonScrollableListView()
            :base(ListViewCachingStrategy.RecycleElement)
        {
        }
    }
}

