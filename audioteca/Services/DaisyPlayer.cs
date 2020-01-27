using audioteca.Helpers;
using audioteca.Models.Player;
using MediaManager;
using MediaManager.Player;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;

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
        private readonly Timer _timer;

        public DaisyPlayer()
        {
            _timer = new Timer(500)
            {
                AutoReset = false
            };
            _timer.Elapsed += PlayCurrentFile;

            CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            CrossMediaManager.Current.MediaItemFinished += Current_MediaItemFinished;
        }

        private void PlayCurrentFile(object sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Code to run on the main thread
                await CrossMediaManager.Current.Play(new FileInfo($"{_playerInfo.Filename}"));
            });
        }

        private async void Current_MediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            if (CrossMediaManager.Current.Position.Ticks != 0)
            {
                await LoadNextFile();
            }
        }

        private void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            _playerInfo.Status = e.State;
            StatusUpdate?.Invoke(e.State);
        }

        private void Current_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
            if (CrossMediaManager.Current.State == MediaPlayerState.Playing)
            {
                _playerInfo.Position.CurrentTC = e.Position.TotalSeconds;

                TimeCodeUpdate?.Invoke(
                    new TimeSpan()
                        .Add(e.Position)
                        .Add(TimeSpan.FromSeconds(_playerInfo.Position.CurrentSOM))
                );
            }

            Debug.WriteLine($"Player: {e.Position.TotalSeconds}, Status: {CrossMediaManager.Current.State}, SOM: {_playerInfo.Position.CurrentSOM}");
        }

        public async void LoadBook(DaisyBook book)
        {
            // Save status of a previous book loaded
            if (_playerInfo != null)
            {
                //await saveStatus(playerInfo);
            }

            _book = book;
            _playerInfo = new PlayerInfo
            {
                Position = new SeekInfo
                {
                    CurrentIndex = 0,
                    NavigationLevel = DaisyBook.NAV_LEVEL_1
                },
            };
            _playerInfo.Filename = $"{AudioBookDataDir.DataDir}/{_book.Id}/{_book.Sequence[_playerInfo.Position.CurrentIndex].Filename}";

            //await loadStatus();
            //await loadBookmarks();
            ChapterUpdate?.Invoke(_book.Title, _book.Sequence[0].Title);
            await CrossMediaManager.Current.Play(new FileInfo($"{_playerInfo.Filename}"));

            //createPlayerControls();
        }

        private async Task<bool> LoadNextFile()
        {
            var currentSeq = _book.Sequence[_playerInfo.Position.CurrentIndex];
            string newFile = null;
            int newIndex = 0;
            for (var i = _playerInfo.Position.CurrentIndex; i < _book.Sequence.Count; i++)
            {
                if (_book.Sequence[i].Filename != currentSeq.Filename)
                {
                    newFile = _book.Sequence[i].Filename;
                    newIndex = i;
                    break;
                }
            }

            if (newFile != null)
            {
                _playerInfo.Filename = $"{AudioBookDataDir.DataDir}/{_book.Id}/{newFile}";
                _playerInfo.Position.CurrentIndex = newIndex;
                _playerInfo.Position.CurrentSOM = _book.Sequence[newIndex].SOM;
                _playerInfo.Position.CurrentTC = 0;
                _playerInfo.Position.CurrentTitle = _book.Sequence[newIndex].Title;

                ChapterUpdate?.Invoke(_book.Title, _playerInfo.Position.CurrentTitle);
                await CrossMediaManager.Current.Play(new FileInfo($"{_playerInfo.Filename}"));

                //await saveStatus(playerInfo);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Play(SeekInfo position)
        {
            if (_playerInfo != null)
            {
                if (CrossMediaManager.Current.IsStopped())
                {
                    CrossMediaManager.Current.PlayPause();
                }

                CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(position.CurrentTC * 1000));

                //musicControls.updateIsPlaying(playerInfo.status == media.MEDIA_RUNNING);
            }
        }

        public async void PlayPause()
        {
            await CrossMediaManager.Current.PlayPause();
        }

        public void Stop()
        {
            if (_playerInfo != null)
            {
                CrossMediaManager.Current.Stop();

                // musicControls.updateIsPlaying(playerInfo.status == media.MEDIA_RUNNING);
            }
        }

        public void Pause()
        {
            if (_playerInfo != null)
            {
                CrossMediaManager.Current.Pause();

                //musicControls.updateIsPlaying(playerInfo.status == media.MEDIA_RUNNING);
            }
        }

        public async Task Move(int updown)
        {
            _timer.Stop();

            await CrossMediaManager.Current.Pause();
            Debug.WriteLine($"Paused");

            // Calculate index position
            var sequence = _book.Sequence.Where(
                                w => w.Filename == Path.GetFileName(_playerInfo.Filename) &&
                                     w.TCIn <= _playerInfo.Position.CurrentTC &&
                                     w.TCOut >= _playerInfo.Position.CurrentTC).FirstOrDefault();
            if (sequence != null)
            {
                _playerInfo.Position.CurrentIndex = _book.Sequence.IndexOf(sequence);
                Debug.WriteLine($"Index by sequence: {_playerInfo.Position.CurrentIndex}");
            }

            var index = _playerInfo.Position.CurrentIndex;

            Debug.WriteLine($"Index Before: {_playerInfo.Position.CurrentIndex}");

            do
            {
                index += updown;
            } while (index > 0 && index < _book.Sequence.Count && _book.Sequence[index].Level > _playerInfo.Position.NavigationLevel);

            // adjust bounds
            if (index < 0)
            {
                index = 0;
            }
            else if (index > _book.Sequence.Count - 1)
            {
                index = _book.Sequence.Count - 1;
            }
            else
            {
                _playerInfo.Position.CurrentIndex = index;
                _playerInfo.Position.CurrentSOM = _book.Sequence[index].SOM;
                _playerInfo.Position.CurrentTC = _book.Sequence[index].TCIn;
                _playerInfo.Position.CurrentTitle = _book.Sequence[index].Title;

                Debug.WriteLine($"Moved Index: {_playerInfo.Position.CurrentIndex}, SOM: {_playerInfo.Position.CurrentSOM}, CurrentTC: {_playerInfo.Position.CurrentTC}");

                ChapterUpdate?.Invoke(_book.Title, _playerInfo.Position.CurrentTitle);
                TimeCodeUpdate?.Invoke(
                    new TimeSpan()
                        .Add(TimeSpan.FromSeconds(_playerInfo.Position.CurrentSOM))
                        .Add(TimeSpan.FromSeconds(_playerInfo.Position.CurrentTC))
                );
            }

            if (_book.Sequence[index].Filename != _playerInfo.Filename)
            {
                _playerInfo.Filename = $"{AudioBookDataDir.DataDir}/{_book.Id}/{_book.Sequence[index].Filename}";
                _timer.Start();
                Debug.WriteLine($"Changed file: {_playerInfo.Filename}");
            }
            else
            {
                if (_playerInfo.Position.CurrentTC > 0)
                {
                    await CrossMediaManager.Current.SeekTo(TimeSpan.FromMilliseconds(_playerInfo.Position.CurrentTC * 1000));
                    Debug.WriteLine($"Seek to: {_playerInfo.Position.CurrentTC * 1000}");
                }
            }

            //await saveStatus(playerInfo);

            //musicControls.updateIsPlaying(playerInfo.status == media.MEDIA_RUNNING);
            //musicControls.updateIsPlaying(playerInfo.status == media.MEDIA_RUNNING);
        }
    }
}
