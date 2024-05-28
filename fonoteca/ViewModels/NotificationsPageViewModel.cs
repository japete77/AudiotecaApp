using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;

namespace fonoteca.ViewModels
{
    public partial class NotificationsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<NotificationModel> items;

        [ObservableProperty]
        private bool showMarkAllRead;

        [ObservableProperty]
        private bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
