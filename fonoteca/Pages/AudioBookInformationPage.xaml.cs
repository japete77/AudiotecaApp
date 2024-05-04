using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class AudioBookInformationPage : ContentPage
{
	public AudioBookInformationPage(AudioBookInformationPageViewModel vm)
	{
		InitializeComponent();
        vm._page = this;
		BindingContext = vm;
	}

    public async Task DeleteBook()
    {
        bool answer = await DisplayAlert("Aviso", "Esto eliminará completamente el audio libro del dispositivo y todos los marcadores asociados ¿desea continuar?", "Si", "No");
        if (answer)
        {
            DaisyPlayer.Instance.Stop();

            var daisyBook = DaisyPlayer.Instance.GetDaisyBook();

            AudioBookStore.Instance.Delete(daisyBook.Id);

            DaisyPlayer.Instance.CleanupPlayerInfo();

            DaisyPlayer.Instance.CleanupDaisyBook();

            await Shell.Current.Navigation.PopToRootAsync(true);            
        }
    }
}