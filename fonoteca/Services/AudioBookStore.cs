using fonoteca.Helpers;
using fonoteca.Models.Api;
using fonoteca.Models.Audiobook;
using Newtonsoft.Json;
using System.IO.Compression;

namespace fonoteca.Services
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
            if (Preferences.ContainsKey(AUDIO_BOOKS_KEY))
            {
                var data = Preferences.Get(AUDIO_BOOKS_KEY, string.Empty);
                if (!string.IsNullOrEmpty(data))
                {
                    _audioBooks = JsonConvert.DeserializeObject<List<MyAudioBook>>(data);
                }
            }

            // Clean up directories
            CleanUp();
        }

        private void SetDownloadCompleted(bool error = false)
        {
            if (_currentAudioBook != null)
            {
                if (!error)
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

        private void SetDownloadProgress(int value)
        {
            if (_currentAudioBook != null)
            {
                _currentAudioBook.Progress = value;
                _currentAudioBook.StatusKey = STATUS_DOWNLOADING;
                _currentAudioBook.StatusDescription = $"{value}% descargado";
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

        private async Task ProcessDownloadQueue()
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
                if (link == null)
                {
                    _currentAudioBook.StatusDescription = "Enlace de descarga no encontrado";
                    _currentAudioBook.StatusKey = STATUS_ERROR;
                    _isProcessingDownload = false;

                    // Remove download from the list
                    int index = _audioBooks.FindIndex(f => f.Book.Id == _currentAudioBook.Book.Id);
                    if (index > -1)
                    {
                        _audioBooks.RemoveAt(index);
                    }

                    SaveBooks();

                    OnProgress?.Invoke(_currentAudioBook);

                    return;
                }

                // download file
                var zipFile = $"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}.zip";

                if (File.Exists(zipFile)) File.Delete(zipFile);

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(link, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                            {
                                long totalBytes = response.Content.Headers.ContentLength ?? -1;
                                long downloadedBytes = 0;
                                byte[] buffer = new byte[8192];
                                int bytesRead;
                                double progress;

                                using (FileStream fileStream = new FileStream(zipFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
                                {
                                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                    {
                                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                                        downloadedBytes += bytesRead;

                                        if (totalBytes > 0)
                                        {
                                            progress = ((double)downloadedBytes / totalBytes) * 100;
                                            SetDownloadProgress((int)progress);
                                        }

                                        if (_cancel)
                                        {
                                            // Remove download from the list
                                            int index = _audioBooks.FindIndex(f => f.Book.Id == _currentAudioBook.Book.Id);
                                            if (index > -1)
                                            {
                                                _audioBooks.RemoveAt(index);
                                            }

                                            SaveBooks();

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    SetDownloadCompleted();
                }
                catch
                {
                    SetDownloadCompleted(true);
                }

                if (_cancel)
                {
                    _cancel = false;
                }
                else
                {
                    _currentAudioBook.StatusDescription = "Preparando audiolibro";
                    _currentAudioBook.StatusKey = STATUS_INSTALLING;
                    _currentAudioBook.TmpFolder = $"{DateTime.Now.Ticks}";
                    SaveBooks();
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
                        _currentAudioBook.StatusKey = STATUS_ERROR;
                        _currentAudioBook.StatusDescription = "Error: no se ha podido extraer el contendido del audiolibro";
                        OnProgress?.Invoke(_currentAudioBook);
                        SaveBooks();
                        return;
                    }


                    // Read daisy format and generate a ncc.json file with all the book content prepared for the audio player
                    var nccIndex = $"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}/ncc.html";
                    if (!File.Exists(nccIndex))
                    {
                        _isProcessingDownload = false;
                        _currentAudioBook.StatusKey = STATUS_ERROR;
                        _currentAudioBook.StatusDescription = "Error: formato del audiolibro incorrecto. Índice no encontrado.";
                        OnProgress?.Invoke(_currentAudioBook);
                        SaveBooks();
                        return;
                    }

                    DaisyBook dbook = new DaisyBook();
                    dbook.Load(nccIndex);
                    dbook.Id = _currentAudioBook.Book.Id;
                    string dbookStr = JsonConvert.SerializeObject(dbook);
                    File.WriteAllText($"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}/ncc.json", dbookStr);

                    // clean up .zip file
                    File.Delete($"{Session.Instance.GetDataDir()}/{_currentAudioBook.Book.Id}.zip");

                    _currentAudioBook.StatusKey = STATUS_COMPLETED;
                    _currentAudioBook.StatusDescription = "Completado";
                    OnProgress?.Invoke(_currentAudioBook);

                    SaveBooks();
                }

                _isProcessingDownload = false;
            }
        }

        private void SaveBooks()
        {
            Preferences.Set(AUDIO_BOOKS_KEY, JsonConvert.SerializeObject(_audioBooks));
        }

        public void Download(AudioBookDetailResult book)
        {
            var newBook = _audioBooks.Where(a => a.Book.Id == book.Id).FirstOrDefault();

            if (newBook == null)
            {
                newBook = new MyAudioBook
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
            }
            else
            {
                newBook.TmpFolder = null;
                newBook.Progress = 0;
                newBook.StatusDescription = "Pendiente de descarga";
                newBook.ErrorCode = 0;
                newBook.StatusKey = STATUS_PENDING;
            }

            OnProgress?.Invoke(newBook);

            // save books
            SaveBooks();

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
                SaveBooks();
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

        public void Delete(string id)
        {
            _audioBooks = _audioBooks.Where(w => w.Book.Id != id).ToList();

            SaveBooks();

            var tmp = $"{Session.Instance.GetDataDir()}/{id}";
            if (Directory.Exists(tmp))
            {
                Directory.Delete(tmp, true);
            }
        }

        private void CleanUp()
        {
            // Calculate books to delete
            var toDelete = _audioBooks.Where(w => w.StatusKey != STATUS_COMPLETED).ToList();

            // Update books with a valid status
            _audioBooks = _audioBooks.Where(w => w.StatusKey == STATUS_COMPLETED).ToList();

            SaveBooks();

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


