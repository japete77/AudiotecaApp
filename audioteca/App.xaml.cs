using audioteca.Helpers;
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
            var isAuthenticated = AsyncHelper.RunSync(() => Session.Instance.IsAuthenticated());
            if (isAuthenticated)
            {
                AsyncHelper.RunSync(() => AudioLibrary.Instance.WarmUp());

                MainPage = new NavigationPage(new MainPage());

                await NotificationsStore.Instance.RegisterUserNotifications();

                await NotificationsStore.Instance.RefreshNotifications();
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
