using fonoteca.Models.Config;
using fonoteca.Services;
using fonoteca.ViewModels;
namespace fonoteca.Pages;

public partial class ConfigurationSpeedPage : ContentPage
{
	private ConfigurationSpeedPageViewModel _vm;

    public ConfigurationSpeedPage(ConfigurationSpeedPageViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		_vm.Items = new List<PlayerSpeed> 
        {
            new PlayerSpeed { Label = "1", Speed = 1.0f },
            new PlayerSpeed { Label = "1.5", Speed = 1.5f },
            new PlayerSpeed { Label = "2", Speed = 2.0f },
            new PlayerSpeed { Label = "2.5", Speed = 2.5f },
            new PlayerSpeed { Label = "3", Speed = 3.0f }
        };
        BindingContext = _vm;
	}

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;
        var index = e.SelectedItemIndex;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        if (DaisyPlayer.HasBeenInitialized)
        {
            DaisyPlayer.Instance.SetSpeed(_vm.Items[index].Speed);
        }        
        Session.Instance.SetSpeed(_vm.Items[index].Speed);
        Session.Instance.SaveSession();

        await Shell.Current.Navigation.PopAsync();
    }

}