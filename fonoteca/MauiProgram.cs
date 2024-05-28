using CommunityToolkit.Maui;
using fonoteca.Pages;
using fonoteca.Services;
using fonoteca.ViewModels;
using fonoteca.Helpers;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;

namespace fonoteca
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureMopups();

            // Register services
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<AudioLibraryPage>();
            builder.Services.AddTransient<AudioLibraryPageViewModel>();
            builder.Services.AddTransient<ByTitlePage>();
            builder.Services.AddTransient<ByTitlePageViewModel>();
            builder.Services.AddTransient<BookDetailsPage>();
            builder.Services.AddTransient<BookDetailsPageViewModel>();
            builder.Services.AddTransient<ByAuthorPage>();
            builder.Services.AddTransient<ByAuthorPageViewModel>();
            builder.Services.AddTransient<ByAuthorTitlesPage>();
            builder.Services.AddTransient<ByAuthorTitlesPageViewModel>();
            builder.Services.AddTransient<MyAudioBooksPage>();
            builder.Services.AddTransient<MyAudioBooksPageViewModel>();
            builder.Services.AddTransient<AudioPlayerPage>();
            builder.Services.AddTransient<AudioPlayerPageViewModel>();
            builder.Services.AddTransient<AudioBookInformationPage>();
            builder.Services.AddTransient<AudioBookInformationPageViewModel>();
            builder.Services.AddTransient<AudioBookIndexPage>();
            builder.Services.AddTransient<AudioBookIndexPageViewModel>();
            builder.Services.AddTransient<BookmarksPage>();
            builder.Services.AddTransient<BookmarksPageViewModel>();
            builder.Services.AddTransient<NavigationLevelsPage>();
            builder.Services.AddTransient<NavigationLevelsPageViewModel>();
            builder.Services.AddTransient<NotificationsPage>();
            builder.Services.AddTransient<NotificationsPageViewModel>();
            builder.Services.AddTransient<NotificationDetailPage>();
            builder.Services.AddTransient<NotificationDetailPageViewModel>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<ChangePasswordPageViewModel>();
            builder.Services.AddTransient<ConfigurationMemoryPage>();
            builder.Services.AddTransient<ConfigurationMemoryPageViewModel>();
            builder.Services.AddTransient<ConfigurationPage>();
            builder.Services.AddTransient<ConfigurationPageViewModel>();
            builder.Services.AddTransient<ConfigurationSpeedPage>();
            builder.Services.AddTransient<ConfigurationSpeedPageViewModel>();
            builder.Services.AddTransient<LoadingPopupViewModel>();
            builder.Services.AddTransient<ILoadingService, LoadingService>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Set the data directory
            var currentDataDir = Session.Instance.GetDataDir();
            if (string.IsNullOrEmpty(currentDataDir) || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                Session.Instance.SetDataDir(AudioBookDataDir.StorageDirs.First().AbsolutePath);
                Session.Instance.SaveSession();
            }

            return builder.Build();
        }
    }
}
