using fonoteca.Helpers;
using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            vm.VersionInfo = AppSettings.Instance.VersionInfo;
            vm.OnlineMode = OfflineChecker.IsConnected;
            BindingContext = vm;

            // Setup data dir if not set
            //var currentDataDir = Session.Instance.GetDataDir();
            //if (string.IsNullOrEmpty(currentDataDir) || DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    Session.Instance.SetDataDir(AudioBookDataDir.StorageDirs.First().AbsolutePath);
            //    Session.Instance.SaveSession();
            //}
        }

        protected override bool OnBackButtonPressed()
        {
            // Logic to close the app
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // For Android
                System.Environment.Exit(0);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // For iOS
                // iOS does not allow programmatic closing of apps
                // Optionally, you can navigate to the first page or pop to root
                Application.Current.Quit();
            }

            // Returning true indicates that you've handled the back button press
            // and prevent the default behavior (navigating back).
            return true;
        }
    }

}
