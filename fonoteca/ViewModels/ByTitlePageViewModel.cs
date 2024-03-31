using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;
using fonoteca.Pages;
using System.Collections.ObjectModel;

namespace fonoteca.ViewModels
{
    public partial class ByTitlePageViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Grouping<string, TitleModel>> items;

        [ObservableProperty]
        bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        private async Task ClickBook(TitleModel item)
        {
            await Shell.Current.Navigation.PushAsync(new BookDetailsPage(new BookDetailsPageViewModel { BookId = item.Id }), true);
        }
    }
}
