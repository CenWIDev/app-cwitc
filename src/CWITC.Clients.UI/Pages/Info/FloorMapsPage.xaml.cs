using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CWITC.Clients.UI
{
    public partial class FloorMapsPage : ContentPage
    {
        public FloorMapsPage()
        {
            InitializeComponent();

			this.Title = "CampusMap";
			this.floorMap.BindingContext = new EvolveMap
			{
				Local = "campus_map.jpg",
				Title = "Campus Map"
			};
        }
    }
}
