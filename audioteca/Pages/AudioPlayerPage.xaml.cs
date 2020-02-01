
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Audiobook;
using audioteca.Models.Player;
using audioteca.Services;
using audioteca.ViewModels;
using MediaManager.Player;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioPlayerPage : ContentPage, INotifyPropertyChanged
    {
        private readonly string _id;
        private DaisyBook _dbook;
        private readonly AudioPlayerPageViewModel _model;

        public AudioPlayerPage(string id)
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            this._id = id;

            _model = new AudioPlayerPageViewModel();

            this.BindingContext = _model;

            _model.Loading = true;
            
            Title = "Reproductor";

            DaisyPlayer.Instance.TimeCodeUpdate += Instance_TimeCodeUpdate;
            DaisyPlayer.Instance.StatusUpdate += Instance_StatusUpdate;
            DaisyPlayer.Instance.ChapterUpdate += Instance_ChapterUpdate;

            InitializeComponent();
        }

        private void Instance_ChapterUpdate(string title, string chapter)
        {
            _model.Title = title;
            _model.Chapter = chapter;
        }

        private void Instance_StatusUpdate(PlayerInfo state)
        {
            if (state == null) return;

            if (state.Status == MediaPlayerState.Playing)
            {
                _model.PlayStopCaption = "Parar";
            }
            else if (state.Status == MediaPlayerState.Paused)
            {
                _model.PlayStopCaption = "Iniciar";
            }

            var currentLevel = DaisyPlayer.Instance.GetNavigationLevels()
                .Where(w => w.Level == state.Position.NavigationLevel)
                .FirstOrDefault();

            if (currentLevel != null)
            {
                _model.NavigationLevel = currentLevel.Label;
            }

            Instance_TimeCodeUpdate(
                new TimeSpan()
                    .Add(TimeSpan.FromSeconds(state.Position.CurrentTC))
                    .Add(TimeSpan.FromSeconds(state.Position.CurrentSOM))
            );

            _model.Chapter = state.Position.CurrentTitle;
        }

        public async void ButtonClick_Backward(object sender, EventArgs e)
        {
            await DaisyPlayer.Instance.Move(-1);
        }

        public void ButtonClick_PlayStop(object sender, EventArgs e)
        {
            DaisyPlayer.Instance.PlayPause();
        }

        public async void ButtonClick_Forward(object sender, EventArgs e)
        {
            await DaisyPlayer.Instance.Move(1);
        }

        private void Instance_TimeCodeUpdate(System.TimeSpan e)
        {
            _model.CurrentTC = string.Format("{0:00}:{1:00}:{2:00} / {3}", e.Hours, e.Minutes, e.Seconds, _dbook.TotalTime);
        }

        protected override void OnAppearing()
        {
            _dbook = DaisyPlayer.Instance.GetDaisyBook();

            if (_dbook == null || _dbook.Id != _id)
            {
                var abookJson = File.ReadAllText($"{AudioBookDataDir.DataDir}/{this._id}/ncc.json");

                _dbook = JsonConvert.DeserializeObject<DaisyBook>(abookJson);

                DaisyPlayer.Instance.LoadBook(_dbook);
            }

            _model.Title = _dbook.Title;

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;

            // Update status
            Instance_StatusUpdate(DaisyPlayer.Instance.GetPlayerInfo());
        }

        public async void ButtonClick_Index(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AudioBookIndexPage(), true);
        }

        public async void ButtonClick_Levels(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationLevelsPage(), true);
        }

        public void ButtonClick_CreateBookmark(object sender, EventArgs e)
        {
        }

        public void ButtonClick_GoToBookmark(object sender, EventArgs e)
        {
        }

        public async void ButtonClick_Info(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AudioBookInformationPage(), true);
        }
    }
}