using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CWITC.Clients.Portable;
using FormsToolkit;

namespace CWITC.Clients.UI
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel vm;
        public SettingsPage ()
        {
            InitializeComponent ();


            BindingContext = vm = new SettingsViewModel ();
            ListViewAbout.HeightRequest = (vm.AboutItems.Count * ListViewAbout.RowHeight);
            ListViewAbout.ItemTapped += (sender, e) => ListViewAbout.SelectedItem = null;
        }

        bool dialogShown;
        int count;
        async void OnTapGestureRecognizerTapped (object sender, EventArgs args)
        {
            count++;
            if (dialogShown || count < 8)
                return;

            dialogShown = true;

            await DisplayAlert ("Credits",
                               "The CWITC mobile apps were created by the Central Wisconsin Developers Group! \n\n" +
                                "Development:\n" +
                                "Drew Frisk\n" +
                                "\n" +
								"Iconography & Color Palette:\n" +
								"Amelia Ruzek\n" +
                                "\n" 
                                +
                                "Special thanks to the Xamarin Evolve mobile app team for creating the original code base."
                                // todo: add any other devs here
                               , "OK");
            
        }
    }
}

