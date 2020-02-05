using Acr.UserDialogs;
using audioteca.Models.Api;
using audioteca.Models.Audiobook;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookDetails : ContentPage
    {
        private readonly string bookId;

        private readonly BookDetailsViewModel _model;

        public BookDetails(string id)
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            this.bookId = id;

            _model = new BookDetailsViewModel
            {
                Loading = true
            };

            this.BindingContext = _model;

            InitializeComponent();

            var book = AudioBookStore.Instance.GetMyAudioBook(id);
            if (book != null)
            {
                Download_OnProgress(book);
                AudioBookStore.Instance.OnProgress += Download_OnProgress;
            }
            else
            {
                _model.ShowCancel = false;
                _model.ShowDownload = true;
                _model.ShowListen = false;
                _model.ShowStatus = false;
            }
        }

        private void Download_OnProgress(Models.Audiobook.MyAudioBook abook)
        {
            if (abook != null && abook.Book.Id == bookId)
            {
                if (abook.StatusKey == AudioBookStore.STATUS_CANCELLED)
                {
                    _model.ShowCancel = false;
                    _model.ShowDownload = true;
                    _model.ShowListen = false;
                    _model.ShowStatus = false;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_DOWNLOADING ||
                         abook.StatusKey == AudioBookStore.STATUS_PENDING)
                {
                    _model.ShowCancel = true;
                    _model.ShowDownload = false;
                    _model.ShowListen = false;
                    _model.ShowStatus = true;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_DOWNLOADED ||
                         abook.StatusKey == AudioBookStore.STATUS_INSTALLING)
                {
                    _model.ShowCancel = false;
                    _model.ShowDownload = false;
                    _model.ShowListen = false;
                    _model.ShowStatus = true;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_ERROR)
                {
                    _model.ShowCancel = false;
                    _model.ShowDownload = true;
                    _model.ShowListen = false;
                    _model.ShowStatus = true;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_COMPLETED)
                {
                    _model.ShowCancel = false;
                    _model.ShowDownload = false;
                    _model.ShowListen = true;
                    _model.ShowStatus = false;
                }

                _model.StatusDescription = abook.StatusDescription;
            }
        }

        protected async override void OnAppearing()
        {
            _model.AudioBook = await AudioLibrary.Instance.GetBookDetail(this.bookId);

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;
        }

        protected override bool OnBackButtonPressed()
        {
            if (_model.ShowCancel)
            {
                UserDialogs.Instance.Confirm(
                    new ConfirmConfig
                    {
                        Title = "Aviso",
                        Message = "Salir de esta pantalla cancelará la descarga ¿desea continuar?",
                        OkText = "Si",
                        CancelText = "No",
                        OnAction = async (action) =>
                        {
                            if (action)
                            {
                                ButtonClick_Cancel(this, null);
                                await this.Navigation.PopAsync();
                            }
                        }
                    }
                );

                return true;
            }

            return false;
        }

        public async void ButtonClick_Download(object sender, EventArgs e)
        {
            AudioBookStore.Instance.OnProgress += Download_OnProgress;
            await AudioBookStore.Instance.Download(_model.AudioBook);
        }

        public void ButtonClick_Cancel(object sender, EventArgs e)
        {
            AudioBookStore.Instance.OnProgress -= Download_OnProgress;

            Download_OnProgress(
                new MyAudioBook
                {
                    StatusKey = AudioBookStore.STATUS_CANCELLED,
                    Book = new AudioBookDetailResult { Id = bookId }
                }
            );

            AudioBookStore.Instance.Cancel(_model.AudioBook.Id);
        }

        public async void ButtonClick_Listen(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new AudioPlayerPage(_model.AudioBook.Id), true);
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

    }
}
