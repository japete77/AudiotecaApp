using audioteca.Helpers;
using audioteca.Models.Player;
using MediaManager;
using MediaManager.Library;
using MediaManager.Player;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static audioteca.ViewModels.NavigationLevelsPageViewModel;

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
        public delegate void OnStatusUpdate(PlayerInfo state);

        public event OnChapterUpdate ChapterUpdate;
        public delegate void OnChapterUpdate(string title, string chapter);

        private PlayerInfo _playerInfo;
        private DaisyBook _book;
        private readonly System.Timers.Timer _timer;
        private bool _seekToCurrentTC = false;

        private const string PLAYER_STATUS_FILE = "status.json";

        public DaisyPlayer()
        {
            _timer = new System.Timers.Timer(500)
            {
                AutoReset = false
            };
            _timer.Elapsed += PlayCurrentFile;

            CrossMediaManager.Current.Volume.CurrentVolume = CrossMediaManager.Current.Volume.MaxVolume;
            CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            CrossMediaManager.Current.MediaItemFinished += Current_MediaItemFinished;
        }

        private void PlayCurrentFile(object sender, System.Timers.ElapsedEventArgs e)
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

        private async void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            if (_seekToCurrentTC && e.State == MediaPlayerState.Playing)
            {
                await CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(_playerInfo.Position.CurrentTC));
                _seekToCurrentTC = false;
            }

            _playerInfo.Status = e.State;
            StatusUpdate?.Invoke(_playerInfo);
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
        }

        public async void LoadBook(DaisyBook book)
        {
            // Save status of a previous book loaded
            if (_playerInfo != null)
            {
                SaveStatus();
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

            LoadStatus();

            ChapterUpdate?.Invoke(_book.Title, _book.Sequence[0].Title);

            await CrossMediaManager.Current.Play(new FileInfo($"{_playerInfo.Filename}"));
            _seekToCurrentTC = true;
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

                SaveStatus();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task Play(SeekInfo position)
        {
            if (_playerInfo != null)
            {
                if (CrossMediaManager.Current.IsStopped())
                {
                    await CrossMediaManager.Current.PlayPause();
                }

                await CrossMediaManager.Current.SeekTo(TimeSpan.FromMilliseconds(position.CurrentTC * 1000));
            }
        }

        public async void PlayPause()
        {
            await CrossMediaManager.Current.PlayPause();
            SaveStatus();
        }

        public async void Stop()
        {
            if (_playerInfo != null)
            {
                await CrossMediaManager.Current.Stop();
                SaveStatus();
            }
        }

        public async void Pause()
        {
            if (_playerInfo != null)
            {
                await CrossMediaManager.Current.Pause();
                SaveStatus();
            }
        }

        public async Task Move(int updown)
        {
            _timer.Stop();

            // Calculate index position
            var sequence = _book.Sequence.Where(
                                w => w.Filename == Path.GetFileName(_playerInfo.Filename) &&
                                     w.TCIn <= _playerInfo.Position.CurrentTC &&
                                     w.TCOut >= _playerInfo.Position.CurrentTC).FirstOrDefault();
            if (sequence != null)
            {
                _playerInfo.Position.CurrentIndex = _book.Sequence.IndexOf(sequence);
            }

            var index = _playerInfo.Position.CurrentIndex;

            do
            {
                index += updown;
            } while (index > 0 && index < _book.Sequence.Count && _book.Sequence[index].Level > _playerInfo.Position.NavigationLevel);

            // adjust bounds
            if (index < 0)
            {
                index = 0;
                return;
            }
            else if (index > _book.Sequence.Count - 1)
            {
                index = _book.Sequence.Count - 1;
                return;
            }
            else
            {
                _playerInfo.Position.CurrentIndex = index;
                _playerInfo.Position.CurrentSOM = _book.Sequence[index].SOM;
                _playerInfo.Position.CurrentTC = _book.Sequence[index].TCIn;
                _playerInfo.Position.CurrentTitle = _book.Sequence[index].Title;

                ChapterUpdate?.Invoke(_book.Title, _playerInfo.Position.CurrentTitle);
                TimeCodeUpdate?.Invoke(
                    new TimeSpan()
                        .Add(TimeSpan.FromSeconds(_playerInfo.Position.CurrentSOM))
                        .Add(TimeSpan.FromSeconds(_playerInfo.Position.CurrentTC))
                );
            }

            await CrossMediaManager.Current.Pause();

            if (_book.Sequence[index].Filename != _playerInfo.Filename)
            {
                _playerInfo.Filename = $"{AudioBookDataDir.DataDir}/{_book.Id}/{_book.Sequence[index].Filename}";
                _timer.Start();
            }
            else
            {
                if (_playerInfo.Position.CurrentTC > 0)
                {
                    await CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(_playerInfo.Position.CurrentTC));
                }
            }

            SaveStatus();
        }

        public async Task Seek(int index, double tc = 0)
        {
            _playerInfo.Position.CurrentIndex = index;
            _playerInfo.Position.CurrentSOM = _book.Sequence[index].SOM;
            _playerInfo.Position.CurrentTitle = _book.Sequence[index].Title;
            _playerInfo.Position.CurrentTC = tc > 0 ? tc :  _book.Sequence[index].TCIn;

            if (_book.Sequence[index].Filename != _playerInfo.Filename)
            {
                _playerInfo.Filename = $"{AudioBookDataDir.DataDir}/{_book.Id}/{_book.Sequence[index].Filename}";
                await CrossMediaManager.Current.Play(new FileInfo($"{_playerInfo.Filename}"));
                _seekToCurrentTC = true;
            }
            else
            {
                if (_playerInfo.Position.CurrentTC > 0)
                {
                    await CrossMediaManager.Current.SeekTo(TimeSpan.FromSeconds(_playerInfo.Position.CurrentTC));
                }
            }
        }

        public void SetLevel(int level)
        {
            if (_playerInfo != null && _playerInfo.Position != null)
            {
                _playerInfo.Position.NavigationLevel = level;
            }
        }

        public PlayerInfo GetPlayerInfo()
        {
            return _playerInfo;
        }

        public DaisyBook GetDaisyBook()
        {
            return _book;
        }

        public void AddBookmark(Bookmark bookmark)
        {
            if (_playerInfo != null)
            {
                if (_playerInfo.Bookmarks == null) _playerInfo.Bookmarks = new List<Bookmark>();
                _playerInfo.Bookmarks.Add(bookmark);
                SaveStatus();
            }
        }

        public void RemoveBookmark(Bookmark bookmark)
        {
            if (_playerInfo != null && _playerInfo.Bookmarks != null)
            {
                _playerInfo.Bookmarks = _playerInfo.Bookmarks.Where(w => w.Id != bookmark.Id).ToList();
                SaveStatus();
            }
        }

        public void SaveStatus()
        {
            File.WriteAllText(
                $"{AudioBookDataDir.DataDir}/{_book.Id}/{PLAYER_STATUS_FILE}",
                JsonConvert.SerializeObject(_playerInfo)
            );
        }

        public void LoadStatus()
        {
            string statusPath = $"{AudioBookDataDir.DataDir}/{_book.Id}/{PLAYER_STATUS_FILE}";
            if (File.Exists(statusPath))
            {
                _playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(File.ReadAllText(statusPath));
            }
        }

        public List<NavigationLevel> GetNavigationLevels()
        {
            var levels = new List<NavigationLevel>();
            if (_book != null)
            {
                for (var i = 1; i <= _book.MaxLevels; i++)
                {
                    levels.Add(new NavigationLevel
                    {
                        Label = $"Nivel {i}",
                        Level = i
                    });
                }

                levels.Add(new NavigationLevel
                {
                    Label = $"Frase",
                    Level = DaisyBook.NAV_LEVEL_PHRASE
                });

                levels.Add(new NavigationLevel
                {
                    Label = $"Página",
                    Level = DaisyBook.NAV_LEVEL_PAGE
                });
            }

            return levels;
        }

    }
}
