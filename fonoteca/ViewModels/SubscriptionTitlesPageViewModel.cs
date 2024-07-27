using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;
using System.Collections.ObjectModel;

namespace fonoteca.ViewModels
{
    public partial class SubscriptionTitlesPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SubscriptionTitle> items;

        [ObservableProperty]
        private bool loading;

        [ObservableProperty]
        private string code;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
