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
        BindingContext = vm;

        if (DaisyPlayer._player == null)
        {
            DaisyPlayer._player = MediaElement;
        }
        else
        {
            MediaElement = DaisyPlayer._player;
        }

        DaisyPlayer.Instance.TimeCodeUpdate += vm.Instance_TimeCodeUpdate;
        DaisyPlayer.Instance.StatusUpdate += vm.Instance_StatusUpdate;
        DaisyPlayer.Instance.ChapterUpdate += vm.Instance_ChapterUpdate;

        vm.Instance_StatusUpdate(DaisyPlayer.Instance.GetPlayerInfo());
    }

    protected override void OnAppearing()
    {
        _vm.Dbook = DaisyPlayer.Instance.GetDaisyBook();

        if (_vm.Dbook == null || _vm.Dbook.Id != _vm.Id)
        {
            if (Directory.Exists($"{Session.Instance.GetDataDir()}/{_vm.Id}") &&
                File.Exists($"{Session.Instance.GetDataDir()}/{_vm.Id}/ncc.json"))
            {
                var abookJson = File.ReadAllText($"{Session.Instance.GetDataDir()}/{_vm.Id}/ncc.json");

                _vm.Dbook = JsonConvert.DeserializeObject<DaisyBook>(abookJson);

                DaisyPlayer.Instance.LoadBook(_vm.Dbook);

                _vm.Title = _vm.Dbook.Title;

                _vm.Loading = false;

                // Update status
                _vm.Instance_StatusUpdate(DaisyPlayer.Instance.GetPlayerInfo());
            }
            else
            {
                // TODO: Alert using MAUI

                //UserDialogs.Instance.Alert(
                //    new AlertConfig
                //    {
                //        Title = "Aviso",
                //        Message = "Audiolibro no encontrado, por favor descárgalo de nuevo.",
                //        OkText = "Aceptar",
                //        OnAction = async () =>
                //        {
                //            await AudioBookStore.Instance.Delete(this._id);

                //            DaisyPlayer.Instance.CleanupPlayerInfo();

                //            DaisyPlayer.Instance.CleanupDaisyBook();

                //            await Navigation.PopToRootAsync(true);
                //        }
                //    }
                //);
            }
        }

        _vm.Loading = false;
    }
}