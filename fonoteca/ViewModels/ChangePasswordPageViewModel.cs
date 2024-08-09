using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Pages;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class ChangePasswordPageViewModel : ObservableObject
    {
        private const string DefaultPassword = "1234";
        readonly ILoadingService _loadingPopup;

        public ChangePasswordPageViewModel(ILoadingService loadingPopup)
        {
            _loadingPopup = loadingPopup;
        }

        [ObservableProperty]
        string newPassword;

        [ObservableProperty]
        string confirmNewPassword;

        [ObservableProperty]
        string errorMessage;

        [RelayCommand]
        public async Task ChangePassword()
        {
            if (!string.IsNullOrEmpty(NewPassword) &&
                NewPassword != DefaultPassword &&
                NewPassword == ConfirmNewPassword)
            {
                try
                {
                    using (await _loadingPopup.Show("Cambiando contraseña"))
                    {
                        await Session.Instance.ChangePassword(NewPassword);
                        Session.Instance.SetPassword(NewPassword);
                        Session.Instance.SaveSession();

                        await Shell.Current.GoToAsync(nameof(MainPage));
                    }
                }
                catch
                {
                    ErrorMessage = "Error cambiando la contraseña";
                }
                finally
                {
                }
            }
            else
            {
                ErrorMessage = "Las contraseñas no coinciden o no son válidas";
            }
        }
    }
}
