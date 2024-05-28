using fonoteca.Models.Player;
using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class NavigationLevelsPage : ContentPage
{
	private readonly NavigationLevelsPageViewModel _vm;

    public NavigationLevelsPage(NavigationLevelsPageViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        _vm.Loading = true;

        _vm.Items = new List<NavigationLevel>();
        var levels = DaisyPlayer.Instance.GetNavigationLevels();
        levels.ForEach(item => _vm.Items.Add(item));

        _vm.Loading = false;
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        var level = (e.SelectedItem as NavigationLevel).Level;

        DaisyPlayer.Instance.SetLevel(level);

        await Shell.Current.Navigation.PopAsync(true);
    }

}