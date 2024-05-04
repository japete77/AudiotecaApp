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
        }
    }
}
