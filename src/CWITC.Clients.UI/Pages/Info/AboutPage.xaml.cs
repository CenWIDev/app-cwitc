﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CWITC.Clients.Portable;
using FormsToolkit;

namespace CWITC.Clients.UI
{
    public partial class AboutPage : ContentPage
    {
        AboutViewModel vm;

        public AboutPage()
        {
            InitializeComponent();
            BindingContext = vm = new AboutViewModel();

            var adjust = Device.RuntimePlatform != "Android" ? 1 : -vm.AboutItems.Count + 1;

            ListViewInfo.HeightRequest = (vm.InfoItems.Count * ListViewInfo.RowHeight) - adjust;
            ListViewInfo.ItemSelected += async (sender, e) => 
                {
                    var item = ListViewInfo.SelectedItem as CWITC.Clients.Portable.MenuItem;
                    if(item == null)
                        return;
                    Page page = null;
                    switch(item.Parameter)
                    {
						case "about":
							App.Logger.TrackPage(AppPage.Settings.ToString());
							page = new SettingsPage();
							break;
                        case "venue":
                            App.Logger.TrackPage(AppPage.Venue.ToString());
                            page = new VenuePage();
                            break;
                        case "sponsors":
                            App.Logger.TrackPage(AppPage.Sponsors.ToString());
                            page = new SponsorsPage();
                            break;
                        case "floor-maps":
                            App.Logger.TrackPage(AppPage.FloorMap.ToString());
                            page = new FloorMapsPage();
                            break;
						case "lunch-locations":
                            App.Logger.TrackPage(AppPage.LunchLocations.ToString());
                            page = new LunchLocationsPage();
							break;
                    }

                    if(page == null)
                        return;

                    if(Device.RuntimePlatform == "iOS" && page is VenuePage)
                        await NavigationService.PushAsync(((Page)this.Parent.Parent).Navigation, page);
                    else
                        await NavigationService.PushAsync(Navigation, page);

                    ListViewInfo.SelectedItem = null;
                };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.UpdateItems();
        }

        public void OnResume()
        {
            OnAppearing();
        }
    }
}

