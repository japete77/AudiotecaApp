using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Try to login
        var isAuthenticated = await Session.Instance.IsAuthenticated();
        if (isAuthenticated)
        {
            // AsyncHelper.RunSync(() => AudioLibrary.Instance.WarmUp());
            await Shell.Current.GoToAsync(nameof(MainPage));

            //await NotificationsStore.Instance.RegisterUserNotifications();

            //await NotificationsStore.Instance.RefreshNotifications();
        }
        else
        {
            ((LoginPageViewModel)BindingContext).IsVisible = true;
        }
    }
}