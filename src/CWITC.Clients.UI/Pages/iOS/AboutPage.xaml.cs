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

            var adjust = Device.OS != TargetPlatform.Android ? 1 : -vm.AboutItems.Count + 1;
            ListViewAbout.HeightRequest = (vm.AboutItems.Count * ListViewAbout.RowHeight) - adjust;
            ListViewAbout.ItemTapped += (sender, e) => ListViewAbout.SelectedItem = null;
            ListViewInfo.HeightRequest = (vm.InfoItems.Count * ListViewInfo.RowHeight) - adjust;

            ListViewAccount.HeightRequest = (vm.AccountItems.Count * ListViewAccount.RowHeight) - adjust;
            ListViewAccount.ItemTapped += (sender, e) => ListViewAccount.SelectedItem = null;;

            ListViewAbout.ItemSelected += async (sender, e) => 
                {
                    if(ListViewAbout.SelectedItem == null)
                        return;

                    App.Logger.TrackPage(AppPage.Settings.ToString());
                    await NavigationService.PushAsync(Navigation, new SettingsPage());

                    ListViewAbout.SelectedItem = null;
                };

            ListViewInfo.ItemSelected += async (sender, e) => 
                {
                    var item = ListViewInfo.SelectedItem as CWITC.Clients.Portable.MenuItem;
                    if(item == null)
                        return;
                    Page page = null;
                    switch(item.Parameter)
                    {
                        case "evaluations":
                            App.Logger.TrackPage ("Evaluations");
                            page = new EvaluationsPage ();
                            break;
                        case "venue":
                            App.Logger.TrackPage(AppPage.Venue.ToString());
                            page = new VenuePage();
                            break;
                        case "code-of-conduct":
                            App.Logger.TrackPage(AppPage.CodeOfConduct.ToString());
                            page = new CodeOfConductPage();
                            break;
                        case "wi-fi":
                            App.Logger.TrackPage(AppPage.WiFi.ToString());
                            page = new WiFiInformationPage();
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
                    if(Device.OS == TargetPlatform.iOS && page is VenuePage)
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

