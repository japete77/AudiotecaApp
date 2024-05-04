using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class AudioBookInformationPage : ContentPage
{
	public AudioBookInformationPage(AudioBookInformationPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}