using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;
using fonoteca.Pages;
using System.Collections.ObjectModel;

namespace fonoteca.ViewModels
{
    public partial class ByAuthorPageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Grouping<string, AuthorModel>> items;

        [ObservableProperty]
        bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        private async Task ClickAuthor(AuthorModel item)
        {
            await Shell.Current.Navigation.PushAsync(new ByAuthorTitlesPage(new ByAuthorTitlesPageViewModel { AuthorId = item.Id }), true);
        }

    }
}
