using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Helpers;

namespace fonoteca.ViewModels
{
    public partial class ConfigurationMemoryPageViewModel : ObservableObject
    {
        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        List<StorageDir> items;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
