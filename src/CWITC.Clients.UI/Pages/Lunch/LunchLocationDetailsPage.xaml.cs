using System;
using System.Collections.Generic;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CWITC.Clients.UI
{
    public partial class LunchLocationDetailsPage : ContentPage
    {
        LunchLocationsDetailViewModel ViewModel => vm ?? (vm = BindingContext as LunchLocationsDetailViewModel);
        LunchLocationsDetailViewModel vm;
        private bool didInit;

        public LunchLocation Location
        {
            get { return ViewModel.Location; }
            set
            {
                BindingContext = new LunchLocationsDetailViewModel(Navigation, value);
                InitView();
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            vm = null;
        }

        public LunchLocationDetailsPage()
        {
            InitializeComponent();
        }

        private void InitView()
        {
            if (!didInit)
            {
                Title = ViewModel.Location.Name;
                if (Device.OS == TargetPlatform.Android)
                {
                    ToolbarItems.Add(new ToolbarItem
                    {
                        Order = ToolbarItemOrder.Secondary,
                        Text = Language.GetDirections,
                        Command = vm.NavigateCommand,
                        Icon = "toolbar_navigate.png"
                    });

					ToolbarItems.Add(new ToolbarItem
					{
						Order = ToolbarItemOrder.Secondary,
                        Text = Language.ViewWebsite,
                        Command = vm.ViewMenuCommand,
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
                else if (Device.OS == TargetPlatform.iOS)
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
                                items = new[] { Language.GetDirections, Language.ViewWebsite };
                            }
                            else
                            {
                                items = new[] { Language.GetDirections, Language.ViewWebsite, string.Format(Language.CallLocationFormat, vm.Location.Name) };
                            }

                            var action = await DisplayActionSheet(vm.Location.Name, Language.Cancel, null, items);
                            if (action == items[0])
                                vm.NavigateCommand.Execute(null);
							if (action == items[1])
                                vm.ViewMenuCommand.Execute(null);
                            else if (items.Length > 2 && action == items[3] && vm.CanMakePhoneCall)
                                vm.CallCommand.Execute(null);
                        })
                    });
                }

                didInit = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (CWITC_Map.Pins.Count > 0)
                return;

            var position = new Position(ViewModel.Location.Latitude, ViewModel.Location.Longitude);
            CWITC_Map.MoveToRegion(new MapSpan(position, 0.02, 0.02));
            CWITC_Map.Pins.Add(new Pin
            {
                Type = PinType.Place,
                Address = $"{ViewModel.Location.Address1} {ViewModel.Location.Address1}",
                Label = ViewModel.Location.Name,
                Position = position
            });
        }

    }
}
