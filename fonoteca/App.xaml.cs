using CommunityToolkit.Maui.Views;
using fonoteca.Services;

namespace fonoteca
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
