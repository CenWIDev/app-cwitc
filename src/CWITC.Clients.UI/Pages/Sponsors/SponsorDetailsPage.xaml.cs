﻿using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using CWITC.Clients.Portable;
using CWITC.DataObjects;

namespace CWITC.Clients.UI
{
    public partial class SponsorDetailsPage : ContentPage
    {
        SponsorDetailsViewModel ViewModel => vm ?? (vm = BindingContext as SponsorDetailsViewModel);
        SponsorDetailsViewModel vm;

        public SponsorDetailsPage()
        {
            InitializeComponent();
           
        }


        public Sponsor Sponsor
        {
            get { return ViewModel.Sponsor; }
            set { BindingContext = new SponsorDetailsViewModel(Navigation, value); }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            vm = null;
            var adjust = Device.RuntimePlatform != "Android" ? 1 : -ViewModel.FollowItems.Count + 1;
            ListViewFollow.HeightRequest = (ViewModel.FollowItems.Count * ListViewFollow.RowHeight) - adjust;
        }
    }
}

