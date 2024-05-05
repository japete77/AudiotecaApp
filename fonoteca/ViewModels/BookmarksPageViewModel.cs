using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Player;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class BookmarksPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<Bookmark> items;

        [ObservableProperty]
        private bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
