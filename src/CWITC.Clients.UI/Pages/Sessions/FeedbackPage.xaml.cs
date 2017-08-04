using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CWITC.DataObjects;
using CWITC.Clients.Portable;

namespace CWITC.Clients.UI
{
    public partial class FeedbackPage : ContentPage
    {
        FeedbackViewModel vm;
        public FeedbackPage(Session session)
        {
            InitializeComponent();

            BindingContext = vm = new FeedbackViewModel(Navigation, session);
            if (Device.OS != TargetPlatform.iOS)
                ToolbarDone.Icon = "toolbar_close.png";


            ToolbarDone.Command = new Command(async () => 
                {
                    if(vm.IsBusy)
                        return;
                    
                    await Navigation.PopModalAsync();
                });
        }
    }
}

