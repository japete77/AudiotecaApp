using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private const string DefaultPassword = "1234";

        readonly ILoadingService loadingPopup;
        public LoginPageViewModel(ILoadingService loadingPopup)
        {
            this.loadingPopup = loadingPopup;
        }

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
            using (await loadingPopup.Show("Comprobando credenciales"))
            {
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
                                //var currentPage = Navigation.NavigationStack.Last();
                                //await Navigation.PushAsync(new ChangePasswordPage(), true);
                                //Navigation.RemovePage(currentPage);
                            }
                            else
                            {
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
            }
        }
    }
}
