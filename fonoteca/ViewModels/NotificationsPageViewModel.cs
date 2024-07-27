using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;
using fonoteca.Services;
using System.Collections.ObjectModel;

namespace fonoteca.ViewModels
{
    public partial class NotificationsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<NotificationModel> items;

        [ObservableProperty]
        private bool showMarkAllRead;

        [ObservableProperty]
        private bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        public void MarkAllRead()
        {
            foreach (var notification in Items)
            {
                if (notification.TextStyle == FontAttributes.Bold)
                {
                    NotificationsStore.Instance.SetNotificationRead(notification.Id);
                    notification.TextStyle = FontAttributes.None;
                    notification.Header = notification.Title;
                }
            }

            ShowMarkAllRead = false;
        }
    }
}
