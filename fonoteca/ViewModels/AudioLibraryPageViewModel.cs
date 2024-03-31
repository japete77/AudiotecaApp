using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Pages;

namespace fonoteca.ViewModels
{
    public partial class AudioLibraryPageViewModel : ObservableObject
    {
        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        public async Task GoToByTitle()
        {
            await Shell.Current.GoToAsync(nameof(ByTitlePage));
        }

        [RelayCommand]
        public async Task GoToByAuthor()
        {
            await Shell.Current.GoToAsync("");
        }
    }
}
