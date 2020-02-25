using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Models.Audiobook;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace audioteca.Services
{
    public class AudioBookStore
    {
        // Singleton implementation
        private static AudioBookStore _instance;
        private bool _cancel;

        public static AudioBookStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AudioBookStore();
                }

                return _instance;
            }
        }

        public static string STATUS_PENDING = "pending";
        public static string STATUS_DOWNLOADING = "downloading";
        public static string STATUS_DOWNLOADED = "downloaded";
        public static string STATUS_INSTALLING = "installing";
        public static string STATUS_CANCELLED = "cancelled";
        public static string STATUS_ERROR = "error";
        public static string STATUS_COMPLETED = "completed";

        // Event to communicate progress
        public delegate void ProgressEventHandler(MyAudioBook abook);
        public event ProgressEventHandler OnProgress;

        private const string AUDIO_BOOKS_KEY = "AudioBooks";
        private List<MyAudioBook> _audioBooks = new List<MyAudioBook>();
        private MyAudioBook _currentAudioBook;
        private bool _isProcessingDownload;

        public AudioBookStore()
        {
            // Read audiobooks list
            Application.Current.Properties.TryGetValue(AUDIO_BOOKS_KEY, out object data);
            if (data != null) _audioBooks = JsonConvert.DeserializeObject<List<MyAudioBook>>(data.ToString());

            // Clean up directories
            AsyncHelper.RunSync(() => CleanUp());
        }

        private void _webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_currentAudioBook != null)
            {
                if (e.Error == null)
                {
                    _currentAudioBook.Progress = 100;
                    _currentAudioBook.StatusKey = STATUS_DOWNLOADED;
                    _currentAudioBook.StatusDescription = $"Descarga completada";
                }
                else
                {
                    _currentAudioBook.Progress = 0;
                    _currentAudioBook.StatusKey = STATUS_ERROR;
                    _currentAudioBook.StatusDescription = $"Error descargando audiolibro";
                    _cancel = true;
                }
                OnProgress?.Invoke(_currentAudioBook);
            }
        }

        private void _webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_currentAudioBook != null)
            {
                _currentAudioBook.Progress = e.ProgressPercentage;
                _currentAudioBook.StatusKey = STATUS_DOWNLOADING;
                _currentAudioBook.StatusDescription = $"{e.ProgressPercentage}% descargado";
                OnProgress?.Invoke(_currentAudioBook);
            }
        }

        private MyAudioBook GetNextDownloadItem()
        {
            foreach (var item in _audioBooks)
            {
                if (item.StatusKey == STATUS_PENDING) return item;
            }

            return null;
        }

        private void ProcessDownloadQueue()
        {
            if (_isProcessingDownload) return;

            _isProcessingDownload = true;

            while ((_currentAudioBook = GetNextDownloadItem()) != null)
            {
                if (_currentAudioBook == null)
                {
                    _isProcessingDownload = false;
                    return;
                }

                _cancel = false;

                var link = AsyncHelper.RunSync(() => AudioLibrary.Instance.GetAudioBookLink(_currentAudioBook.Book.Id));
                if (link == null) return;

                // download file
                var zipFile = $"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}.zip";

                if (File.Exists(zipFile)) File.Delete(zipFile);

                // Initialize web client
                var webClient = new WebClient();
                webClient.DownloadProgressChanged += _webClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += _webClient_DownloadFileCompleted;
                webClient.DownloadFileAsync(new Uri(link.AudioBookLink), zipFile);

                while (webClient.IsBusy)
                {
                    if (_cancel)
                    {
                        // Remove download from the list
                        int index = _audioBooks.FindIndex(f => f.Book.Id == _currentAudioBook.Book.Id);
                        if (index > -1)
                        {
                            _audioBooks.RemoveAt(index);
                        }

                        AsyncHelper.RunSync(() => SaveBooks());

                        webClient.CancelAsync();

                        break;
                    }

                    Thread.Sleep(100);
                }

                if (_cancel)
                {
                    _cancel = false;
                }
                else
                {
                    _currentAudioBook.StatusDescription = "Preparando audiolibro";
                    _currentAudioBook.StatusKey = STATUS_INSTALLING;
                    _currentAudioBook.TmpFolder = $"{DateTime.Now.Ticks.ToString()}";
                    AsyncHelper.RunSync(() => SaveBooks());
                    OnProgress?.Invoke(_currentAudioBook);

                    try
                    {
                        // Unzip
                        using (ZipArchive archive = ZipFile.OpenRead(zipFile))
                        {
                            double totalBytes = archive.Entries.Sum(e => e.Length);
                            long currentBytes = 0;

                            var targetPath = $"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}";

                            // clean up if exists
                            if (Directory.Exists(targetPath)) Directory.Delete(targetPath, true);

                            Directory.CreateDirectory(targetPath);

                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (!string.IsNullOrEmpty(entry.Name))
                                {
                                    string fileName = Path.Combine($"{targetPath}", Path.GetFileName(entry.FullName));

                                    using (Stream inputStream = entry.Open())
                                    using (Stream outputStream = File.OpenWrite(fileName))
                                    {
                                        Stream progressStream = new StreamWithProgress(outputStream, null,
                                            new BasicProgress<int>(i =>
                                            {
                                                currentBytes += i;
                                                _currentAudioBook.Progress = (int)(currentBytes / totalBytes);
                                                OnProgress?.Invoke(_currentAudioBook);
                                            })
                                        );

                                        inputStream.CopyTo(progressStream);
                                    }

                                    File.SetLastWriteTime(fileName, entry.LastWriteTime.LocalDateTime);
                                }
                            }
                        }
                    }
                    catch
                    {
                        _isProcessingDownload = false;
                        return;
                    }

                    // Read daisy format and generate a ncc.json file with all the book content prepared for the audio player
                    DaisyBook dbook = new DaisyBook();
                    dbook.Load($"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}/ncc.html");
                    dbook.Id = _currentAudioBook.Book.Id;
                    string dbookStr = JsonConvert.SerializeObject(dbook);
                    File.WriteAllText($"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}/ncc.json", dbookStr);

                    // clean up .zip file
                    File.Delete($"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}.zip");

                    _currentAudioBook.StatusKey = STATUS_COMPLETED;
                    _currentAudioBook.StatusDescription = "Completado";
                    OnProgress?.Invoke(_currentAudioBook);

                    AsyncHelper.RunSync(() => SaveBooks());
                }

                _isProcessingDownload = false;
            }
        }

        private async Task SaveBooks()
        {
            Application.Current.Properties[AUDIO_BOOKS_KEY] = JsonConvert.SerializeObject(_audioBooks);
            await Application.Current.SavePropertiesAsync();
        }

        public async Task Download(AudioBookDetailResult book)
        {
            MyAudioBook newBook = new MyAudioBook
            {
                Book = book,
                Path = $"{Session.Instance.GetDataDir()}",
                Filename = $"{book.Id}.zip",
                TmpFolder = null,
                Progress = 0,
                StatusDescription = "Pendiente de descarga",
                ErrorCode = 0,
                StatusKey = STATUS_PENDING
            };

            _audioBooks.Add(newBook);

            OnProgress?.Invoke(newBook);

            // save books
            await SaveBooks();

            // process download queue
#pragma warning disable 4014
            Task.Run(() => ProcessDownloadQueue());
#pragma warning restore 4014
        }

        public void Cancel(string id)
        {
            var index = _audioBooks.FindIndex(value => value.Book.Id == id);
            if (index > -1)
            {
                _audioBooks.RemoveAt(index);
                AsyncHelper.RunSync(() => SaveBooks());
            }

            if (_currentAudioBook != null &&
                _currentAudioBook.Book.Id == id)
            {
                _cancel = true;
            }

            OnProgress?.Invoke(new MyAudioBook { Book = new AudioBookDetailResult { Id = id }, StatusKey = STATUS_CANCELLED });
        }

        public MyAudioBook GetMyAudioBook(string id)
        {
            return _audioBooks.Find(value => value.Book.Id == id);        
        }

        public List<MyAudioBook> GetMyAudioBooks()
        {
            return _audioBooks;
        }

        public async Task Delete(string id)
        {
            _audioBooks = _audioBooks.Where(w => w.Book.Id != id).ToList();

            await SaveBooks();

            var tmp = $"{Session.Instance.GetDataDir()}/{id}";
            if (Directory.Exists(tmp))
            {
                Directory.Delete(tmp, true);
            }
        }

        private async Task CleanUp()
        {
            // Calculate books to delete
            var toDelete = _audioBooks.Where(w => w.StatusKey != STATUS_COMPLETED).ToList();

            // Update books with a valid status
            _audioBooks = _audioBooks.Where(w => w.StatusKey == STATUS_COMPLETED).ToList();

            await SaveBooks();

            // Clean up folders
            toDelete.ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item.Path))
                {
                    var tmp = $"{Session.Instance.GetDataDir()}/{item.Filename}";
                    if (File.Exists(tmp))
                    {
                        File.Delete(tmp);
                    }

                    tmp = $"{Session.Instance.GetDataDir()}/{item.Book.Id}";
                    if (Directory.Exists(tmp))
                    {
                        Directory.Delete(tmp, true);
                    }
                }

                if (!string.IsNullOrEmpty(item.TmpFolder))
                {
                    var tmp = $"{Session.Instance.GetDataDir()}/{item.TmpFolder}";
                    if (Directory.Exists(tmp))
                    {
                        Directory.Delete(tmp, true);
                    }
                }
            });
        }
    }
}


