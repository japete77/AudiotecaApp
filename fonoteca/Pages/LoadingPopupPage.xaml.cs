using fonoteca.ViewModels;
using Mopups.Pages;

namespace fonoteca.Pages;

public partial class LoadingPopupPage : PopupPage
{
	public LoadingPopupPage(LoadingPopupViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}