using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using Xamarin.Forms;

namespace audioteca
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected async override void OnStart()
        {
            // Try to login
            var isAuthenticated = await Session.Instance.IsAuthenticated();
            if (isAuthenticated)
            {
                await AudioLibrary.Instance.WarmUp();

                MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
