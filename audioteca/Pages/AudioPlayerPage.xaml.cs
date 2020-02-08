
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Player;
using audioteca.Services;
using audioteca.ViewModels;
using MediaManager.Player;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioPlayerPage : ContentPage
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
            else if (state.Status == MediaPlayerState.Paused || state.Status == MediaPlayerState.Stopped)
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

        public async void ButtonClick_PlayStop(object sender, EventArgs e)
        {
            var info = DaisyPlayer.Instance.GetPlayerInfo();
            if (info.Status == MediaPlayerState.Stopped)
            {
                await DaisyPlayer.Instance.Play(info.Position);
            }
            else
            {
                DaisyPlayer.Instance.PlayPause();
            }
        }

        public async void ButtonClick_Forward(object sender, EventArgs e)
        {
            await DaisyPlayer.Instance.Move(1);
        }

        private void Instance_TimeCodeUpdate(System.TimeSpan e)
        {
            _model.CurrentTC = $"{GetTimeCode(e)} / {_dbook.TotalTime}";
        }

        private string GetTimeCode(System.TimeSpan e)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", e.Hours, e.Minutes, e.Seconds);
        }

        protected override void OnAppearing()
        {
            _dbook = DaisyPlayer.Instance.GetDaisyBook();

            if (_dbook == null || _dbook.Id != _id)
            {
                var abookJson = File.ReadAllText($"{Session.Instance.GetDataDir()}/{this._id}/ncc.json");

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
            var info = DaisyPlayer.Instance.GetPlayerInfo();

            int counter = 1;
            if (info.Bookmarks != null && info.Bookmarks.Count > 0)
            {
                for (var i = 0; i < info.Bookmarks.Count; i++)
                {
                    if (counter < info.Bookmarks[i].Id)
                    {
                        counter = info.Bookmarks[i].Id;
                    }
                }
                counter++;
            }

            var bookmark = new Bookmark
            {
                Id = counter,
                Index = info.Position.CurrentIndex,
                Title = $"Marcador {counter}",
                TC = info.Position.CurrentTC,
                SOM = info.Position.CurrentSOM,
                AbsoluteTC = GetTimeCode(
                                TimeSpan.FromSeconds(info.Position.CurrentSOM)
                                        .Add(TimeSpan.FromSeconds(info.Position.CurrentTC))
                            )
            };

            UserDialogs.Instance.Prompt(
                new PromptConfig
                {
                    Title = "Crear marcador",
                    Message = $"Marcador en {bookmark.AbsoluteTC}",
                    OkText = "Crear",
                    CancelText = "Cancelar",
                    Placeholder = "Marcador",
                    Text = bookmark.Title,
                    OnAction = (action) =>
                    {
                        if (action.Ok)
                        {
                            bookmark.Title = action.Text;
                            DaisyPlayer.Instance.AddBookmark(bookmark);
                        }
                    }
                }
            );
        }

        public async void ButtonClick_GoToBookmark(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BookmarksPage(), true);
        }

        public async void ButtonClick_Info(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AudioBookInformationPage(), true);
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

    }
}