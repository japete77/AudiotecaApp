﻿using CommunityToolkit.Maui;
using fonoteca.Pages;
using fonoteca.Services;
using fonoteca.ViewModels;
using fonoteca.Helpers;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Microsoft.Maui.LifecycleEvents;
#if ANDROID
using Plugin.Firebase.Bundled.Platforms.Android;
#endif
#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Firebase.Core;
using UIKit;
using UserNotifications;
#endif
#if ANDROID || IOS
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.Crashlytics;
using Firebase;
#endif
using System.Diagnostics;

namespace fonoteca
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterFirebaseServices()
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
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ForgotPasswordPageViewModel>();
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
            builder.Services.AddTransient<SubscriptionsPage>();
            builder.Services.AddTransient<SubscriptionsPageViewModel>();
            builder.Services.AddTransient<SubscriptionTitlesPage>();
            builder.Services.AddTransient<SubscriptionTitlesPageViewModel>();
            builder.Services.AddTransient<SubscriptionTitleDetailsPage>();
            builder.Services.AddTransient<SubscriptionTitleDetailsPageViewModel>();
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
            if (string.IsNullOrEmpty(currentDataDir) || !Directory.Exists(currentDataDir))
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    Session.Instance.SetDataDir(AudioBookDataDir.StorageDirs.First().AbsolutePath);
                    Session.Instance.SaveSession();
                }
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    Session.Instance.SetDataDir(AudioBookDataDir.StorageDirs.First().AbsolutePath);
                    Session.Instance.SaveSession();
                }
            }
            return builder.Build();
        }

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            if (!OfflineChecker.IsConnected) return builder;

            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                InitializeFirebaseAndroid();

                events.AddAndroid(android => android.OnCreate((activity, _) =>
                    CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings()))
                );
                CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);
#elif IOS
                InitializeFirebaseiOS();

                events.AddiOS(ios => ios.FinishedLaunching((application, launchOptions) =>
                {
                    // Asigna el delegado para manejar notificaciones
                    UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();

                    var center = UNUserNotificationCenter.Current;

                    // Obtener la configuración actual de notificaciones
                    center.GetNotificationSettings((settings) =>
                    {
                        // Si el usuario ya autorizó las notificaciones, no se solicita nuevamente.
                        if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized ||
                            settings.AuthorizationStatus == UNAuthorizationStatus.Provisional)
                        {
                            Console.WriteLine("Los permisos de notificaciones ya han sido otorgados.");
                        }
                        else
                        {
                            // En caso de que aún no se hayan otorgado los permisos, se solicitan.
                            var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                            center.RequestAuthorization(authOptions, (granted, error) =>
                            {
                                if (granted)
                                {
                                    Console.WriteLine("Permisos de notificaciones concedidos.");
                                }
                                else
                                {
                                    Console.WriteLine("Permisos de notificaciones denegados.");
                                }
                            });
                        }
                    });

                    // Registrar la app para recibir notificaciones remotas
                    application.RegisterForRemoteNotifications();

                    // Enable Crashlytics collection after Firebase initialization
                    CrossFirebaseCrashlytics.Current.SetCrashlyticsCollectionEnabled(true);

                    return true;
                }));
#endif

            });

#if ANDROID || IOS
            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
#endif
            return builder;
        }

#if ANDROID
        private static void InitializeFirebaseAndroid()
        {
            // Retrieve the FirebaseOptions from API
            var config = NotificationsStore.Instance.GetFirebaseAndroidConfig();

            if (config != null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetApiKey(config.ApiKey)
                    .SetApplicationId(config.ApplicationId)
                    .SetProjectId(config.ProjectId)
                    .SetStorageBucket(config.StorageBucket)
                    .Build();

                // Assuming MainPage.Context is defined, or use another context provider
                var context = Android.App.Application.Context;

                FirebaseApp.InitializeApp(context, options);
            }
        }
#endif

#if IOS
        private static void InitializeFirebaseiOS()
        {
            // Recupera la configuración para iOS
            var config = NotificationsStore.Instance.GetFirebaseiOSConfig();

            if (config != null)
            {
                // Crea las opciones para Firebase. El constructor requiere el ApplicationId (GOOGLE_APP_ID)
                // y el GcmSenderId.
                var options = new Options(config.ApplicationId, config.GcmSenderId)
                {
                    ApiKey = config.ApiKey,
                    ProjectId = config.ProjectId,
                    StorageBucket = config.StorageBucket
                };

                // Configura Firebase con las opciones creadas
                Firebase.Core.App.Configure(options);
            }
        }
#endif

#if ANDROID || IOS
        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            return new CrossFirebaseSettings(
                isAuthEnabled: true, 
                isCloudMessagingEnabled: true, 
                isAnalyticsEnabled: true
            );
        }
#endif

    }
}
