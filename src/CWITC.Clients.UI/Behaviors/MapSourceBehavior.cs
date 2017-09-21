using System;
using System.Linq;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.Collections.Specialized;
using CWITC.Clients.Portable;

namespace CWITC.Clients.UI
{
    public class MapBehavior : BindableBehavior<Map>
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create<MapBehavior, IEnumerable<ILocationViewModel>>(p => p.ItemsSource, null, BindingMode.Default, null, ItemsSourceChanged);

        public static readonly BindableProperty ShowDetailCommandProperty = BindableProperty.Create<MapBehavior, ICommand>(x => x.ShowDetailCommand, null);

        public static readonly BindableProperty ShouldAdjustPanProperty = BindableProperty.Create<MapBehavior, bool>(x => x.ShowAdjustPan, false);

        public IEnumerable<ILocationViewModel> ItemsSource
        {
            get { return (IEnumerable<ILocationViewModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public ICommand ShowDetailCommand
        { 
            get{ return (ICommand)base.GetValue(ShowDetailCommandProperty); }
            set{ base.SetValue(ShowDetailCommandProperty, value); }
        }

        public bool ShowAdjustPan
        {
            get{ return (bool)base.GetValue(ShouldAdjustPanProperty); }
            set{ base.SetValue(ShouldAdjustPanProperty, value); }
        }

        private static void ItemsSourceChanged(BindableObject bindable, IEnumerable oldValue, IEnumerable newValue)
        {
            var behavior = bindable as MapBehavior;
            if (behavior == null)
                return;

            if (oldValue is INotifyCollectionChanged)
            {
                (oldValue as INotifyCollectionChanged).CollectionChanged -= behavior.ItemSourceCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged)
            {
                (newValue as INotifyCollectionChanged).CollectionChanged += behavior.ItemSourceCollectionChanged;
            }

            behavior.AddPins();
        }

        void ItemSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.AddPins();
        }

        private void AddPins()
        {
            var map = AssociatedObject;
            for (int i = map.Pins.Count - 1; i >= 0; i--)
            {
                map.Pins[i].Clicked -= PinOnClicked;
                map.Pins.RemoveAt(i);
            }

            var pins = ItemsSource.Select(x =>
                {
                    var pin = new Pin
                    {
                        Type = PinType.SearchResult,
                        Position = new Position(x.Latitude, x.Longitude),
                        Label = x.Title,
                        Address = x.Description,

                    };

                    pin.Clicked += PinOnClicked;
                    return pin;
                }).ToArray();

            foreach (var pin in pins)
            {
                if (string.IsNullOrEmpty(pin.Label) == false)
                {
                    map.Pins.Add(pin);
                }
            }

            PositionMap();
        }

        private void PinOnClicked(object sender, EventArgs eventArgs)
        {
            var pin = sender as Pin;
            if (pin == null)
                return;
            var viewModel = ItemsSource.FirstOrDefault(x => x.Title == pin.Label);
            if (viewModel == null)
                return;

            if (this.ShowDetailCommand != null && this.ShowDetailCommand.CanExecute(viewModel))
            {
                this.ShowDetailCommand.Execute(viewModel);
            }
        }

        private void PositionMap()
        {
            if (ItemsSource == null || !ItemsSource.Any())
                return;

            var centerPosition = new Position(ItemsSource.Average(x => x.Latitude), ItemsSource.Average(x => x.Longitude));

            var minLongitude = ItemsSource.Min(x => x.Longitude);
            var minLatitude = ItemsSource.Min(x => x.Latitude);

            var maxLongitude = ItemsSource.Max(x => x.Longitude);
            var maxLatitude = ItemsSource.Max(x => x.Latitude);

            var distance = MapHelper.CalculateDistance(minLatitude, minLongitude, maxLatitude, maxLongitude, 'M') / 2;

            if (this.ShowAdjustPan == true)
            {
                try
                {
                    AssociatedObject.MoveToRegion(MapSpan.FromCenterAndRadius(centerPosition, Distance.FromMiles(distance)));
                }
                catch (Exception)
                {
                    AssociatedObject.MoveToRegion(MapSpan.FromCenterAndRadius(centerPosition, Distance.FromMiles(0.5)));
                }

                Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
                    {
                        AssociatedObject.MoveToRegion(MapSpan.FromCenterAndRadius(centerPosition, Distance.FromMiles(distance)));
                        return false;
                    });
            }
        }
    }

	public class MapHelper
	{
		//15 = 1/2, 14 = 1, 13 = 2, 12 = 4, 11 = 8, 10 = 16
		//-1 = 1/2, 0 = 1, 1 = 2, 2 = 4, 3 = 8, 4 = 16
		//2^(14-zoom) = radius
		//ln(x^y) = y*ln(x)
		//ln(2^(14-zoom)) = ln(radius) = (14-zoom) * ln(2)
		//ln(radius)/ln(2) -14 = -zoom
		//zoom = 14 - ln(radius)/ln(2)
		//throw  round on there to make sure we get an integer
		//and don't make the g-maps API angry
		public static double RadiusToZoom(double radius)
		{

			return Math.Round(14 - Math.Log(radius) / Math.Log(2));/// 0.693);

			//          return Math.round(14-Math.log(radius)/Math.LN2);
		}

		public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2, char unit)
		{
			double theta = lon1 - lon2;
			double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
			dist = Math.Acos(dist);
			dist = Rad2Deg(dist);
			dist = dist * 60 * 1.1515;
			if (unit == 'K')
			{
				dist = dist * 1.609344;
			}
			else if (unit == 'N')
			{
				dist = dist * 0.8684;
			}
			return (dist);
		}

		private static double Deg2Rad(double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		private static double Rad2Deg(double rad)
		{
			return (rad / Math.PI * 180.0);
		}
	}
}