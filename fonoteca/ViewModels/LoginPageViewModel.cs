using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private const string DefaultPassword = "1234";

        [ObservableProperty]
        bool isVisible = false;

        [ObservableProperty]
        bool mustChangePassword;

        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        string errorMessage;

        [RelayCommand]
        async Task Login()
        {
            // UserDialogs.Instance.ShowLoading("Comprobando credenciales");

            try
            {
                if (int.TryParse(Username, out int iUsername))
                {
                    var result = await Session.Instance.Login(iUsername, Password);
                    if (result)
                    {
                        // Warm up audio books
                        await AudioLibrary.Instance.WarmUp();

                        if (Password == DefaultPassword)
                        {
                            // Go to ChangePassword page
                            // UserDialogs.Instance.HideLoading();

                            //var currentPage = Navigation.NavigationStack.Last();
                            //await Navigation.PushAsync(new ChangePasswordPage(), true);
                            //Navigation.RemovePage(currentPage);
                        }
                        else
                        {
                            // Go to Main page
                            //UserDialogs.Instance.HideLoading();

                            await Shell.Current.GoToAsync(nameof(MainPage));

                            //var currentPage = Navigation.NavigationStack.Last();
                            //await Navigation.PushAsync(new MainPage(), true);
                            //Navigation.RemovePage(currentPage);
                        }

                        //await NotificationsStore.Instance.RegisterUserNotifications();

                        //await NotificationsStore.Instance.RefreshNotifications();
                    }
                    else
                    {
                        ErrorMessage = "Usuario o contraseña incorrectos";
                    }
                }
                else
                {
                    ErrorMessage = "Usuario o contraseña incorrectos";
                }
            }
            catch
            {
                ErrorMessage = "Fonoteca no disponible";
            }

            // UserDialogs.Instance.HideLoading();
        }
    }
}
