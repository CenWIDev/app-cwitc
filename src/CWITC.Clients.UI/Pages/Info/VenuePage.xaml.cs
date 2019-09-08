using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using CWITC.Clients.Portable;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace CWITC.Clients.UI
{
    public partial class VenuePage : ContentPage
    {
        VenueViewModel vm;
        public VenuePage()
        {
            InitializeComponent();
            BindingContext = vm = new VenueViewModel();

            if (Device.RuntimePlatform == "Android")
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Order = ToolbarItemOrder.Secondary,
                    Text = Language.GetDirections,
                    Command = vm.NavigateCommand

                });

                if (vm.CanMakePhoneCall)
                {

                    ToolbarItems.Add(new ToolbarItem
                    {
                        Order = ToolbarItemOrder.Secondary,
                        Text = Language.CallVenue,
                        Command = vm.CallCommand
                    });
                }
            }
            else if (Device.RuntimePlatform == "iOS")
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = Language.More,
                    Icon = "toolbar_overflow.png",
                    Command = new Command(async () =>
                        {
                            string[] items = null;
                            if (!vm.CanMakePhoneCall)
                            {
                                items = new[] { Language.GetDirections };
                            }
                            else
                            {
                                items = new[] { Language.GetDirections, Language.CallVenue };
                            }

                        var action = await DisplayActionSheet(Language.VenueName, Language.Cancel, null, items);
                            if (action == items[0])
                                vm.NavigateCommand.Execute(null);
                            else if (items.Length > 1 && action == items[1] && vm.CanMakePhoneCall)
                                vm.CallCommand.Execute(null);

                        })
                });
            }
            else
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = Language.GetDirections,
                    Command = vm.NavigateCommand,
                    Icon = "toolbar_navigate.png"
                });

                if (vm.CanMakePhoneCall)
                {

                    ToolbarItems.Add(new ToolbarItem
                    {
                        Text = Language.CallVenue,
                        Command = vm.CallCommand,
                        Icon = "toolbar_call.png"
                    });
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (CWITC_Map.Pins.Count > 0)
                return;

            var position = new Position(vm.Latitude, vm.Longitude);
            CWITC_Map.MoveToRegion(new MapSpan(position, 0.02, 0.02));
            CWITC_Map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Address = vm.LocationTitle,
                Label = vm.EventTitle,
                Position = position
            });
        }


    }
}

