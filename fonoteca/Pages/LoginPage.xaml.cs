using fonoteca.Helpers;
using fonoteca.Services;
using fonoteca.ViewModels;
using Plugin.Firebase.CloudMessaging;

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

        using (await _loading.Show("Cargando"))
        {
            // Try to login
            var isAuthenticated = await Session.Instance.IsAuthenticated();
            if (isAuthenticated)
            {
                await AudioLibrary.Instance.WarmUp();

                await Shell.Current.GoToAsync(nameof(MainPage));

#if ANDROID || IOS
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                NotificationsStore.Instance.SaveDeviceToken(token);

                await NotificationsStore.Instance.RegisterUserNotifications();

                await NotificationsStore.Instance.RefreshNotifications();
#endif
            }
            else
            {
                ((LoginPageViewModel)BindingContext).IsVisible = true;
            }
        }
    }
}