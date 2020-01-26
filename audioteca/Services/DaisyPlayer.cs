using audioteca.Helpers;
using audioteca.Models.Player;
using MediaManager;
using MediaManager.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace audioteca.Services
{
    public class DaisyPlayer
    {
        // Singleton implementation
        private static DaisyPlayer _instance;

        public static DaisyPlayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DaisyPlayer();
                }

                return _instance;
            }
        }

        public event OnTimeCodeUpdate TimeCodeUpdate;
        public delegate void OnTimeCodeUpdate(TimeSpan e);

        public event OnStatusUpdate StatusUpdate;
        public delegate void OnStatusUpdate(MediaPlayerState state);

        public event OnChapterUpdate ChapterUpdate;
        public delegate void OnChapterUpdate(string title, string chapter);

        private PlayerInfo _playerInfo;
        private DaisyBook _book;

        public DaisyPlayer()
        {
            CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            CrossMediaManager.Current.MediaItemFinished += Current_MediaItemFinished;
        }

        private async void Current_MediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            await LoadNextFile();
        }

        private void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            StatusUpdate?.Invoke(e.State);
        }

        private void Current_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
            TimeCodeUpdate?.Invoke(
                new TimeSpan()
                    .Add(e.Position)
                    .Add(TimeSpan.FromSeconds(this._playerInfo.Position.CurrentSOM))
            );
        }

        public async void LoadBook(DaisyBook book)
        {
            // Save status of a previous book loaded
            if (this._playerInfo != null)
            {
                //await this.saveStatus(this.playerInfo);
            }

            this._playerInfo = new PlayerInfo();
            this._playerInfo.Position = new SeekInfo();
            this._playerInfo.Position.CurrentIndex = 0;
            // this._playerInfo.Position.NavigationLevel = NAV_LEVEL_1;
            this._playerInfo.Status = MediaPlayerState.Stopped;

            this._book = book;

            //await this.loadStatus();
            //await this.loadBookmarks();
            var file = $"{AudioBookDataDir.DataDir}/{this._book.Id}/{this._book.Sequence[this._playerInfo.Position.CurrentIndex].Filename}";
            await CrossMediaManager.Current.Play(new FileInfo($"{file}"));

            ChapterUpdate?.Invoke(this._book.Title, this._book.Sequence[0].Title);

            //this.createPlayerControls();
        }

        private async Task<bool> LoadNextFile()
        {
            var currentSeq = this._book.Sequence[this._playerInfo.Position.CurrentIndex];
            string newFile = null;
            int newIndex = 0;
            for (var i = this._playerInfo.Position.CurrentIndex; i < this._book.Sequence.Count; i++)
            {
                if (this._book.Sequence[i].Filename != currentSeq.Filename)
                {
                    newFile = this._book.Sequence[i].Filename;
                    newIndex = i;
                    break;
                }
            }

            if (newFile != null)
            {
                this._playerInfo.Position.CurrentIndex = newIndex;
                this._playerInfo.Position.CurrentSOM = this._book.Sequence[newIndex].SOM;
                this._playerInfo.Position.CurrentTC = 0;
                this._playerInfo.Position.AbsoluteTC = this.Seconds2TC(this._playerInfo.Position.CurrentSOM + this._playerInfo.Position.CurrentTC);
                this._playerInfo.Position.CurrentTitle = this._book.Sequence[newIndex].Title;

                ChapterUpdate?.Invoke(this._book.Title, this._playerInfo.Position.CurrentTitle);

                string filetoplay = $"{AudioBookDataDir.DataDir}/{this._book.Id}/{newFile}";
                await CrossMediaManager.Current.Play(new FileInfo($"{filetoplay}"));
                //await this.saveStatus(this.playerInfo);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Play(SeekInfo position)
        {
            if (this._playerInfo != null)
            {
                if (CrossMediaManager.Current.IsStopped())
                {
                    CrossMediaManager.Current.PlayPause();
                }
                CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(position.CurrentTC * 1000));
                this._playerInfo.Status = MediaPlayerState.Playing;
                //this.musicControls.updateIsPlaying(this.playerInfo.status == this.media.MEDIA_RUNNING);
            }
        }

        public void PlayPause()
        {
            CrossMediaManager.Current.PlayPause();
        }

        private string Seconds2TC(float seconds)
        {
            if (seconds < 0) seconds = 0;

            return Math.Floor(seconds / 3600).ToString() + ":" +
                   Math.Floor((seconds / 60) % 60).ToString().PadLeft(2, '0') + ":" +
                   Math.Floor(seconds % 60).ToString().PadLeft(2, '0');
        }
    }
}
