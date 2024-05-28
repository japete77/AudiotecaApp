using fonoteca.ViewModels;
using fonoteca.Helpers;

namespace fonoteca.Pages;

public partial class ConfigurationPage : ContentPage
{
	private ConfigurationPageViewModel _vm;

    public ConfigurationPage(ConfigurationPageViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		_vm.Page = this;
		_vm.HasExternalMemory = AudioBookDataDir.StorageDirs.Count > 1;
        BindingContext = _vm;
	}
}