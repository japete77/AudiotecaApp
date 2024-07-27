using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;
using System.Collections.ObjectModel;

namespace fonoteca.ViewModels
{
    public partial class SubscriptionsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Subscription> items;

        [ObservableProperty]
        private bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
