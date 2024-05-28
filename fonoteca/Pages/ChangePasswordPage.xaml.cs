using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class ChangePasswordPage : ContentPage
{
	private ChangePasswordPageViewModel _vm;

    public ChangePasswordPage(ChangePasswordPageViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = _vm;
	}
}