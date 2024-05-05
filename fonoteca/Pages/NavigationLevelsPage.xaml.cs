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
}