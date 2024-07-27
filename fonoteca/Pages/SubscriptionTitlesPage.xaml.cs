using fonoteca.Models.Api;
using fonoteca.Services;
using fonoteca.ViewModels;
using System.Collections.ObjectModel;

namespace fonoteca.Pages;

public partial class SubscriptionTitlesPage : ContentPage
{
    private readonly SubscriptionTitlesPageViewModel _vm;
    private ILoadingService _loading;    

	public SubscriptionTitlesPage(SubscriptionTitlesPageViewModel vm)
	{
        InitializeComponent();
        vm.Items = new ObservableCollection<SubscriptionTitle>();
        vm.Loading = true;
        _vm = vm;
        BindingContext = vm;
        _loading = Application.Current.Handler.MauiContext.Services.GetService<ILoadingService>();
    }

    protected async override void OnAppearing()
    {
        if (_vm.Loading)
        {
            using (await _loading.Show("Cargando"))
            {
                var subscriptionTitles = await AudioLibrary.Instance.GetSubscriptionTitles(_vm.Code);

                if (subscriptionTitles == null)
                {
                    return;
                }

                // Clear the existing items before adding new ones
                _vm.Items.Clear();
                subscriptionTitles.ForEach(item => _vm.Items.Add(item));

                _vm.Loading = false;
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Reset selected items of the view
        SubscriptionTitlesView.SelectedItem = null;
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        var selected = e.SelectedItem as SubscriptionTitle;

        await Shell.Current.Navigation.PushAsync(
            new SubscriptionTitleDetailsPage(
                new SubscriptionTitleDetailsPageViewModel
                {
                    BookId = $"{_vm.Code}{selected.Id}",
                    AudioBook = new AudioBookDetailResult
                    {
                        Id = $"{_vm.Code}{selected.Id}",
                        Title = selected.Title,
                    },
                    Title = selected,
                }
            ),
            true
        );
    }
}