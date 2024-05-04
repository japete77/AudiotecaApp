using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Player;
using fonoteca.Pages;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class AudioPlayerPageViewModel : ObservableObject
    {
        public string Id;

        public DaisyBook Dbook;

        [ObservableProperty]
        bool loading = false;

        [ObservableProperty]
        string currentTC;

        [ObservableProperty]
        string playStopCaption;

        [ObservableProperty]
        string title;

        [ObservableProperty]
        string chapter;

        [ObservableProperty]
        string navigationLevel;

        public void Instance_TimeCodeUpdate(TimeSpan e)
        {
            CurrentTC = $"{GetTimeCode(e)} / {Dbook?.TotalTime}";
        }

        private string GetTimeCode(TimeSpan e)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", e.Hours, e.Minutes, e.Seconds);
        }

        public void Instance_StatusUpdate(PlayerInfo state)
        {
            if (state == null) return;

            if (state.Status == MediaElementState.Playing)
            {
                PlayStopCaption = "Parar";
            }
            else if (state.Status == MediaElementState.Paused || state.Status == MediaElementState.Stopped)
            {
                PlayStopCaption = "Iniciar";
            }

            var currentLevel = DaisyPlayer.Instance.GetNavigationLevels()
                .Where(w => w.Level == state.Position.NavigationLevel)
                .FirstOrDefault();

            if (currentLevel != null)
            {
                NavigationLevel = currentLevel.Label;
            }

            Instance_TimeCodeUpdate(
                new TimeSpan()
                    .Add(TimeSpan.FromSeconds(state.Position.CurrentTC))
                    .Add(TimeSpan.FromSeconds(state.Position.CurrentSOM))
            );

            Chapter = state.Position.CurrentTitle;
        }

        public void Instance_ChapterUpdate(string title, string chapter)
        {
            Title = title;
            Chapter = chapter;
        }

        [RelayCommand]
        public void Backward()
        {
            DaisyPlayer.Instance.Move(-1);
        }

        [RelayCommand]
        public void PlayStop()
        {
            var info = DaisyPlayer.Instance.GetPlayerInfo();
            if (info.Status == MediaElementState.Stopped || info.Status == MediaElementState.None)
            {
                DaisyPlayer.Instance.Play(info.Position);
            }
            else
            {
                DaisyPlayer.Instance.PlayPause();
            }
        }

        [RelayCommand]
        public void Forward()
        {
            DaisyPlayer.Instance.Move(1);
        }

        [RelayCommand]
        public async Task Index()
        {
            await Task.CompletedTask;
            // await Navigation.PushAsync(new AudioBookIndexPage(), true);
        }

        [RelayCommand]
        public async Task Levels()
        {
            await Task.CompletedTask;
            // await Navigation.PushAsync(new NavigationLevelsPage(), true);
        }

        [RelayCommand]
        public void CreateBookmark()
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

            // TODO: Review how to create user dialog in MAUI

            //UserDialogs.Instance.Prompt(
            //    new PromptConfig
            //    {
            //        Title = "Crear marcador",
            //        Message = $"Marcador en {bookmark.AbsoluteTC}",
            //        OkText = "Crear",
            //        CancelText = "Cancelar",
            //        Placeholder = "Marcador",
            //        Text = bookmark.Title,
            //        OnAction = (action) =>
            //        {
            //            if (action.Ok)
            //            {
            //                bookmark.Title = action.Text;
            //                DaisyPlayer.Instance.AddBookmark(bookmark);
            //            }
            //        }
            //    }
            //);
        }

        [RelayCommand]
        public async Task GoToBookmark()
        {
            await Task.CompletedTask;
            // await Navigation.PushAsync(new BookmarksPage(), true);
        }

        [RelayCommand]
        public async Task Info()
        {
            var daisyBook = DaisyPlayer.Instance.GetDaisyBook();
            if (daisyBook != null)
            {
                await Shell.Current.Navigation.PushAsync(
                    new AudioBookInformationPage(
                        new AudioBookInformationPageViewModel 
                        {
                            Charset = daisyBook.Charset,
                            Creator = daisyBook.Creator,
                            Date = daisyBook.Date,
                            Format = daisyBook.Format,
                            Generator = daisyBook.Generator,
                            Id = daisyBook.Id,
                            Identifier = daisyBook.Identifier,
                            Narrator = daisyBook.Narrator,
                            Producer = daisyBook.Producer,
                            Publisher = daisyBook.Publisher,
                            Source = daisyBook.Source,
                            Subject = daisyBook.Subject,
                            Title = daisyBook.Title,
                            TotalTime = daisyBook.TotalTime,
                        }
                    ), 
                    true
                );
            }
        }

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
