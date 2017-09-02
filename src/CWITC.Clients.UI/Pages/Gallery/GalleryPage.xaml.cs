using System;
using System.Collections.Generic;
using CWITC.Clients.Portable;
using Xamarin.Forms;

namespace CWITC.Clients.UI
{
    public partial class GalleryPage : ContentPage
    {
        public GalleryPage()
        {
            InitializeComponent();

            BindingContext = new GalleryViewModel(this.Navigation);
        }
    }
}
