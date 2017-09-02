using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;
using MvvmHelpers;
using FormsToolkit;

namespace CWITC.Clients.Portable
{
    public class GalleryViewModel : ViewModelBase
    {
        private ICommand _openGrouveCommand;
        public ICommand OpenGrouveCommand => _openGrouveCommand ?? (_openGrouveCommand = new Command(ExecuteOpenGrouveCommand));

        public string GrouveCode { get; private set; }

        public GalleryViewModel(INavigation navigation) : base(navigation)
        {
            GrouveCode = Settings.Current.GrouveEventCode;
            Settings.Current.PropertyChanged += (sender, e) => GrouveCode = Settings.Current.GrouveEventCode;
        }

        private void ExecuteOpenGrouveCommand()
        {
            string url = null;
            if (Device.OS == TargetPlatform.iOS)
                url = "https://itunes.apple.com/us/app/grouve/id1222443822";
            else if (Device.OS == TargetPlatform.Android)
                url = "https://play.google.com/store/apps/details?id=tech.grouve";

            Device.OpenUri(new Uri(url));
        }
    }
}
