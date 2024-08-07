using fonoteca.Models.Api;
using fonoteca.Services;
using fonoteca.ViewModels;
using System.Collections.ObjectModel;

namespace fonoteca.Pages;

public partial class NotificationsPage : ContentPage
{
    private readonly NotificationsPageViewModel _vm;
    private ILoadingService _loading;

    public NotificationsPage(NotificationsPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        vm.Items = new ObservableCollection<NotificationModel>();
        vm.Loading = true;
        _vm = vm;
        _loading = Application.Current.Handler.MauiContext.Services.GetService<ILoadingService>();
    }

    protected async override void OnAppearing()
    {
        if (_vm.Loading)
        {
            using (await _loading.Show("Cargando"))
            {
                var notifications = await NotificationsStore.Instance.GetNotifications();

                if (notifications == null)
                {
                    return;
                }

                // Clear the existing items before adding new ones
                _vm.Items.Clear();
                notifications.ForEach(item => _vm.Items.Add(item));

                _vm.ShowMarkAllRead = notifications.Where(w => w.TextStyle == FontAttributes.Bold).Any();

                _vm.Loading = false;
            }
        }
        else
        {
            var tmp = _vm.Items.ToList();
            _vm.Items.Clear();
            tmp.ForEach(item => _vm.Items.Add(item));
            _vm.ShowMarkAllRead = tmp.Where(w => w.TextStyle == FontAttributes.Bold).Any();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Reset selected items of the notifications view
        NotificationsView.SelectedItem = null;
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        var selected = e.SelectedItem as NotificationModel;

        await Shell.Current.Navigation.PushAsync(
            new NotificationDetailPage(
                new NotificationDetailPageViewModel 
                { 
                    Notification = selected 
                }
            ), 
            true
        );
    }
}