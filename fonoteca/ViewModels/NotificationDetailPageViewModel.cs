using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace fonoteca.ViewModels
{
    public partial class NotificationDetailPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

    }
}
