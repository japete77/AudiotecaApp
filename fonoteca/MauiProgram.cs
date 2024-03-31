using fonoteca.Pages;
using fonoteca.ViewModels;
using Microsoft.Extensions.Logging;

namespace fonoteca
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

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
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
