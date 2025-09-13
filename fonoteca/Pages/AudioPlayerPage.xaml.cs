using fonoteca.Models.Player;
using fonoteca.Services;
using fonoteca.ViewModels;
using Newtonsoft.Json;

namespace fonoteca.Pages;

public partial class AudioPlayerPage : ContentPage
{
    private readonly AudioPlayerPageViewModel _vm;

    public AudioPlayerPage(AudioPlayerPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        _vm._page = this;
        BindingContext = vm;

        DaisyPlayer.Instance.TimeCodeUpdate += vm.Instance_TimeCodeUpdate;
        DaisyPlayer.Instance.StatusUpdate += vm.Instance_StatusUpdate;
        DaisyPlayer.Instance.ChapterUpdate += vm.Instance_ChapterUpdate;

        vm.Instance_StatusUpdate(DaisyPlayer.Instance.GetPlayerInfo());
    }

    protected override async void OnAppearing()
    {
        _vm.Dbook = DaisyPlayer.Instance.GetDaisyBook();

        if (_vm.Dbook == null || _vm.Dbook.Id != _vm.Id)
        {
            if (Directory.Exists($"{Session.Instance.GetDataDir()}{Path.DirectorySeparatorChar}{_vm.Id}") &&
                File.Exists($"{Session.Instance.GetDataDir()}{Path.DirectorySeparatorChar}{_vm.Id}{Path.DirectorySeparatorChar}ncc.json"))
            {
                var abookJson = File.ReadAllText($"{Session.Instance.GetDataDir()}{Path.DirectorySeparatorChar}{_vm.Id}{Path.DirectorySeparatorChar}ncc.json");

                _vm.Dbook = JsonConvert.DeserializeObject<DaisyBook>(abookJson);

                DaisyPlayer.Instance.LoadBook(_vm.Dbook);

                _vm.Title = _vm.Dbook.Title;

                _vm.Loading = false;

                // Update status
                _vm.Instance_StatusUpdate(DaisyPlayer.Instance.GetPlayerInfo());
            }
            else
            {
                await DisplayAlert("Aviso", "Audiolibro no encontrado, por favor descárgalo de nuevo.", "Aceptar");

                AudioBookStore.Instance.Delete(_vm.Id);
                DaisyPlayer.Instance.CleanupPlayerInfo();
                DaisyPlayer.Instance.CleanupDaisyBook();
                await Navigation.PopToRootAsync(true);
            }
        }

        _vm.Loading = false;
    }

    public async Task CreateBookmark(Bookmark bookmark)
    {
        string result = await DisplayPromptAsync("Crear marcador", $"Marcador en {bookmark.AbsoluteTC}", "Crear", "Cancelar", "Marcador", initialValue: bookmark.Title);
        if (!string.IsNullOrEmpty(result))
        {
            bookmark.Title = result;
            DaisyPlayer.Instance.AddBookmark(bookmark);
        }
    }
}