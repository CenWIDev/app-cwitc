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


        public FeedbackPage()
        {
            InitializeComponent();

            var row = (TextEditor.Parent.Parent as StackLayout);
            row.SizeChanged += (sender, e) => 
            {
                var frame = TextEditor.Bounds;
                frame.Height = row.Height - 1;
                TextEditor.Layout(frame);
            };

            if (Device.RuntimePlatform != "iOS")
                ToolbarDone.Icon = "toolbar_close.png";


            ToolbarDone.Command = new Command(async () => 
                {
                    if(vm.IsBusy)
                        return;
                    
                    await Navigation.PopModalAsync();
                });
        }

		public FeedbackPage WithSession(Session session)
		{
			BindingContext = vm = new FeedbackViewModel(Navigation, session);
			
			return this;
		}
    }
}

