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

            CarouselMaps.ItemsSource = new List<EvolveMap>
            {
                new EvolveMap
                {
                    Local = "campus_map.jpg",
                    Title = "Campus Maps"
                }
            };

            if (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
            {
                CarouselMaps.ItemSelected += (sender, args) =>
                {
                    var current = args.SelectedItem as EvolveMap;
                    if (current == null)
                        return;
                    Title = current.Title;
                };
            }
        }
    }
}
