using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Audiobook;
using fonoteca.Pages;
using System.Collections.ObjectModel;

namespace fonoteca.ViewModels
{
    public partial class MyAudioBooksPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Grouping<string, MyAudioBook>> items;

        [ObservableProperty]
        bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        private async Task ClickBook(MyAudioBook item)
        {
            await Shell.Current.Navigation.PushAsync(new AudioPlayerPage(new AudioPlayerPageViewModel { Id = item.Book.Id }), true);
        }
    }
}
