using fonoteca.Models.Api;
using fonoteca.Services;
using fonoteca.ViewModels;
using System.Collections.ObjectModel;

namespace fonoteca.Pages;

public partial class SubscriptionsPage : ContentPage
{
    private readonly SubscriptionsPageViewModel _vm;
    private ILoadingService _loading;

    public SubscriptionsPage(SubscriptionsPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        vm.Items = new ObservableCollection<Subscription>();
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
                var subscriptions = await AudioLibrary.Instance.GetUserSubscriptions(true);

                if (subscriptions == null)
                {
                    return;
                }

                // Clear the existing items before adding new ones
                _vm.Items.Clear();
                subscriptions.Subscriptions
                    .OrderBy(o => o.Description)
                    .ToList()
                    .ForEach(item => _vm.Items.Add(item));

                _vm.Loading = false;
            }
        }
        else
        {
            var tmp = _vm.Items.ToList();
            _vm.Items.Clear();
            tmp.ForEach(item => _vm.Items.Add(item));
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Reset selected items of the notifications view
        SubscriptionsView.SelectedItem = null;        
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        var selected = e.SelectedItem as Subscription;

        await Shell.Current.Navigation.PushAsync(
            new SubscriptionTitlesPage(new SubscriptionTitlesPageViewModel { Code = selected.Code }),
            true
        );
    }
}