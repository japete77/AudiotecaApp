using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using audioteca.Models.Api;
using audioteca.Models.Audiobook;
using audioteca.Services;
using audioteca.ViewModels;
using Xamarin.Forms;

namespace audioteca
{
    public partial class SubscriptionTitleDetailsPage : ContentPage
    {
        private readonly SubscriptionTitle _title;
        private readonly string _code;

        private readonly SubscriptionTitleDetailsViewModel _model;

        public SubscriptionTitleDetailsPage(SubscriptionTitle title, string code)
        {
            NavigationPage.SetHasNavigationBar(this, false);

            this._title = title;
            this._code = code;

            _model = new SubscriptionTitleDetailsViewModel
            {
                Loading = true,
                SubscriptionTitle = title
            };

            this.BindingContext = _model;

            InitializeComponent();

            var book = AudioBookStore.Instance.GetMyAudioBook($"{_code}{_title.Id}");
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
            if (abook != null && abook.Book.Id == $"{_code}{_title.Id}")
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

        protected override void OnAppearing()
        {
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
            await AudioBookStore.Instance.Download(new AudioBookDetailResult
            {
                Id = $"{_code}{_title.Id}",
                Title = _title.Title,
                Comments = _title.Description
            });
        }

        public void ButtonClick_Cancel(object sender, EventArgs e)
        {
            AudioBookStore.Instance.OnProgress -= Download_OnProgress;

            Download_OnProgress(
                new MyAudioBook
                {
                    StatusKey = AudioBookStore.STATUS_CANCELLED,
                    Book = new AudioBookDetailResult { Id = $"{_code}{_title.Id}" }
                }
            );

            AudioBookStore.Instance.Cancel($"{_code}{_title.Id}");
        }

        public async void ButtonClick_Listen(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AudioPlayerPage($"{_code}{_title.Id}"), true);
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            if (!OnBackButtonPressed())
            {
                await Navigation.PopAsync();
            }
        }
    }
}
