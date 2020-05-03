using audioteca.Helpers;
using audioteca.Services;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();

            // Setup data dir if not set
            var currentDataDir = Session.Instance.GetDataDir();
            if (string.IsNullOrEmpty(currentDataDir) || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                Session.Instance.SetDataDir(AudioBookDataDir.StorageDirs.First().AbsolutePath);
                Session.Instance.SaveSession();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        public async void ButtonClick_MyAudiobooks(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new MyAudioBooksPage(), true);
        }

        public async void ButtonClick_Audiobooks(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new AudioLibraryPage(), true);
        }

        public void ButtonClick_Notificacions(object sender, EventArgs e)
        {
        }

        public async void ButtonClick_Configuration(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new ConfigurationPage(), true);
        }
    }
}
