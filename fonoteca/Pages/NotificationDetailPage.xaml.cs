using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class NotificationDetailPage : ContentPage
{
    NotificationDetailPageViewModel _vm;

    public NotificationDetailPage(NotificationDetailPageViewModel vm)
	{
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        // Mark as read
        vm.Notification.TextStyle = FontAttributes.None;
        vm.Notification.Header = "";
        NotificationsStore.Instance.SetNotificationRead(vm.Notification.Id);
    }
}