using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage(ForgotPasswordPageViewModel vm)
	{
		InitializeComponent();        
		vm.IsVisible = true;
        vm.Page = this;
        BindingContext = vm;
    }
}