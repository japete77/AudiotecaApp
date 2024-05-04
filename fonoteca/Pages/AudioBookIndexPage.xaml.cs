using fonoteca.Models.Daisy;
using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class AudioBookIndexPage : ContentPage
{
    private readonly AudioBookIndexPageViewModel _vm = new AudioBookIndexPageViewModel();
	public AudioBookIndexPage(AudioBookIndexPageViewModel vm)
	{
		InitializeComponent();
        _vm = vm;
		BindingContext = vm;
        _vm.Loading = true;
    }

    protected override void OnAppearing()
    {
        var book = DaisyPlayer.Instance.GetDaisyBook();

        if (book != null)
        {
            int selectedLevel = DaisyPlayer.Instance.GetPlayerInfo().Position.NavigationLevel;
            _vm.Items = book.Body.Where(w => w.Level <= selectedLevel).ToList();
        }

        _vm.Loading = false;
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var book = DaisyPlayer.Instance.GetDaisyBook();

        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        var selectedItem = (e.SelectedItem as SmilInfo);

        var selectedSequence = book.Sequence.Where(w => w.Id == selectedItem.Id).First();

        await DaisyPlayer.Instance.Seek(book.Sequence.IndexOf(selectedSequence));

        await Shell.Current.Navigation.PopAsync(true);
    }

}