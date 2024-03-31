using fonoteca.Helpers;
using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            vm.VersionInfo = AppSettings.Instance.VersionInfo;
            BindingContext = vm;

            // Setup data dir if not set
            //var currentDataDir = Session.Instance.GetDataDir();
            //if (string.IsNullOrEmpty(currentDataDir) || DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    Session.Instance.SetDataDir(AudioBookDataDir.StorageDirs.First().AbsolutePath);
            //    Session.Instance.SaveSession();
            //}
        }
    }

}
