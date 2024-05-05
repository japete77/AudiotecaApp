using fonoteca.Models.Player;
using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class BookmarksPage : ContentPage
{
    private readonly BookmarksPageViewModel _vm;
    public BookmarksPage(BookmarksPageViewModel vm)
    {
        _vm = vm;
        _vm.Loading = true;
        BindingContext = vm;

        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        var playerInfo = DaisyPlayer.Instance.GetPlayerInfo();

        if (playerInfo != null && playerInfo.Bookmarks != null)
        {
            _vm.Items = new List<Bookmark>();
            playerInfo.Bookmarks.ForEach(item => _vm.Items.Add(item));
        }

        _vm.Loading = false;
    }

    public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // has been set to null, do not 'process' tapped event
        if (e.SelectedItem == null) return;

        // de-select the row
        ((ListView)sender).SelectedItem = null;

        var selectedItem = (e.SelectedItem as Bookmark);

        await DaisyPlayer.Instance.Seek(selectedItem.Index, selectedItem.TC);

        await Shell.Current.Navigation.PopAsync(true);
    }

    public async void ButtonClick_Delete(object sender, EventArgs e)
    {
        var button = (sender as Button);
        int id = (int)button.CommandParameter;

        var playerInfo = DaisyPlayer.Instance.GetPlayerInfo();
        var toDelete = playerInfo.Bookmarks.Where(w => w.Id == id).FirstOrDefault();
        if (toDelete != null)
        {
            bool answer = await DisplayAlert("Aviso", $"Esto eliminará el marcador {toDelete.Title} ¿desea continuar?", "Si", "No");
            if (answer)
            {
                DaisyPlayer.Instance.RemoveBookmark(toDelete);
                _vm.Items.Remove(toDelete);
            }
        }
    }
}