using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Pages;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class ConfigurationPageViewModel : ObservableObject
    {
        public ConfigurationPage Page;

        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        bool hasExternalMemory;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        public async Task ResetCredentials()
        {
            if (Page != null)
            {
                var response = await Page.DisplayAlert(
                    "Aviso", 
                    "Esto limpiará las credenciales de usuario y tendrá que iniciar sesión de nuevo con un usuario y contraseña ¿desea continuar?", 
                    "Si", 
                    "No"
                );

                if (response)
                {
                    Session.Instance.CleanCredentials();
                    Session.Instance.SaveSession();
                    await Shell.Current.Navigation.PopToRootAsync(true);
                }
            }
        }

        [RelayCommand]
        public async Task SelectSpeed()
        {
            await Shell.Current.Navigation.PushAsync(new ConfigurationSpeedPage(new ConfigurationSpeedPageViewModel()), true);
        }

        [RelayCommand]
        public async Task SelectStorage()
        {
            await Shell.Current.Navigation.PushAsync(new ConfigurationMemoryPage(new ConfigurationMemoryPageViewModel()), true);
        }
    }
}
