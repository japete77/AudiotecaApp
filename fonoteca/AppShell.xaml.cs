using fonoteca.Pages;

namespace fonoteca
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AudioLibraryPage), typeof(AudioLibraryPage));
            Routing.RegisterRoute(nameof(ByTitlePage), typeof(ByTitlePage));
            Routing.RegisterRoute(nameof(ByAuthorPage), typeof(ByAuthorPage));
            Routing.RegisterRoute(nameof(ByAuthorTitlesPage), typeof(ByAuthorTitlesPage));
            Routing.RegisterRoute(nameof(MyAudioBooksPage), typeof(MyAudioBooksPage));
            Routing.RegisterRoute(nameof(AudioPlayerPage), typeof(AudioPlayerPage));
            Routing.RegisterRoute(nameof(AudioBookInformationPage), typeof(AudioBookInformationPage));
            Routing.RegisterRoute(nameof(AudioBookIndexPage), typeof(AudioBookIndexPage));
            Routing.RegisterRoute(nameof(BookmarksPage), typeof(BookmarksPage));
            Routing.RegisterRoute(nameof(NavigationLevelsPage), typeof(NavigationLevelsPage));
            Routing.RegisterRoute(nameof(ConfigurationPage), typeof(ConfigurationPage));
            Routing.RegisterRoute(nameof(ConfigurationSpeedPage), typeof(ConfigurationSpeedPage));
            Routing.RegisterRoute(nameof(ConfigurationMemoryPage), typeof(ConfigurationMemoryPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(LoadingPopupPage), typeof(LoadingPopupPage));
            Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));
            Routing.RegisterRoute(nameof(NotificationDetailPage), typeof(NotificationDetailPage));
            Routing.RegisterRoute(nameof(SubscriptionsPage), typeof(SubscriptionsPage));
            Routing.RegisterRoute(nameof(SubscriptionTitlesPage), typeof(SubscriptionTitlesPage));
            Routing.RegisterRoute(nameof(SubscriptionTitleDetailsPage), typeof(SubscriptionTitleDetailsPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
        }
    }
}
