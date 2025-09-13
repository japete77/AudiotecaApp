
using fonoteca.Services;
using fonoteca.ViewModels;
using fonoteca.Helpers;
#if IOS
using static UIKit.UIGestureRecognizer;
using Firebase.CloudMessaging;
#endif
#if ANDROID || IOS
using Plugin.Firebase.CloudMessaging;
#endif

namespace fonoteca.Pages;

public partial class LoginPage : ContentPage
{
    private ILoadingService _loading;
    public LoginPage(LoginPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _loading = Application.Current.Handler.MauiContext.Services.GetService<ILoadingService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!OfflineChecker.HasInternetAccessFlag)
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
            return;
        }

#if ANDROID || IOS
        using (await _loading.Show("Cargando"))
#endif
        {

#if ANDROID
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            NotificationsStore.Instance.SaveDeviceToken(token);
#endif

#if IOS
            var deviceToken = BitConverter.ToString(Messaging.SharedInstance.ApnsToken.ToArray()).Replace("-", "");
            NotificationsStore.Instance.SaveDeviceToken(deviceToken);
#endif

            // Try to login
            var isAuthenticated = await Session.Instance.IsAuthenticated();
            if (isAuthenticated)
            {
                await AudioLibrary.Instance.WarmUp();

                await Shell.Current.GoToAsync(nameof(MainPage));

                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();

                await NotificationsStore.Instance.RegisterUserNotifications();

                await NotificationsStore.Instance.RefreshNotifications();
            }
            else
            {
                ((LoginPageViewModel)BindingContext).IsVisible = true;
            }
        }
    }
}