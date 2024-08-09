using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Pages;
using fonoteca.Services;
using System.Text.RegularExpressions;

namespace fonoteca.ViewModels
{
    public partial class ForgotPasswordPageViewModel : ObservableObject
    {
        public ForgotPasswordPage Page;

        readonly ILoadingService loadingPopup;
        public ForgotPasswordPageViewModel(ILoadingService loadingPopup)
        {
            this.loadingPopup = loadingPopup;
        }

        [ObservableProperty]
        bool isVisible = false;

        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string errorMessage;

        [RelayCommand]
        async Task RecoverPassword()
        {
            if (!ValidEmail(Email))
            {
                ErrorMessage = "El email no es válido";
                return;
            }
            else
            {
                ErrorMessage = string.Empty;
            }

            using (await loadingPopup.Show("Solicitando nueva contraseña"))
            {
                // Call API to start forgot password workflow
                await Session.Instance.ForgotPassword(Email);
            }

            // Popup message
            await Page.DisplayAlert(
                    "Aviso",
                    $"Se ha enviado un correo electrónico a {Email} con la nueva contraseña. En el próximo acceso se le solicitará el cambio de contraseña.",
                    "Aceptar"
                );

            // Back to login page
            await Shell.Current.Navigation.PopAsync(true);
        }

        bool ValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Expresión regular para validar el email
            string patronEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(patronEmail);

            return regex.IsMatch(email);
        }
    }
}
