using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class AudioLibraryPage : ContentPage
{
	public AudioLibraryPage(AudioLibraryPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}