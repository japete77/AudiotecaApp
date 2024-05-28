using fonoteca.Models.Api;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class NotificationsPage : ContentPage
{
    private readonly NotificationsPageViewModel _vm;

    public NotificationsPage(NotificationsPageViewModel vm)
	{
		InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {        
        try
        {
            _vm.Loading = true;
            //_notifications = await NotificationsStore.Instance.GetNotifications();

            //if (_notifications == null)
            //{
            //    return;
            //}
        }
        catch
        {

        }
        finally
        {
            _vm.Loading = false;                 
        }
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        //await Navigation.PushAsync(
        //    new NotificationDetailPage(
        //        e.SelectedItem as NotificationModel,
        //        e.SelectedItemIndex
        //    ),
        //    true
        //);
    }

}