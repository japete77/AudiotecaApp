using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Config;

namespace fonoteca.ViewModels
{
    public partial class ConfigurationSpeedPageViewModel : ObservableObject
    {
        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        List<PlayerSpeed> items;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
