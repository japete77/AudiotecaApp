﻿using Acr.UserDialogs;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace audioteca
{
    public partial class AudioLibraryPage : ContentPage, INotifyPropertyChanged
    {
        private readonly AudioLibraryPageViewModel _model;

        public AudioLibraryPage()
        {
            UserDialogs.Instance.ShowLoading("Verificando credenciales");

            _model = new AudioLibraryPageViewModel();
            this.BindingContext = _model;
            _model.Loading = true;
            Title = "Audioteca";
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            _model.Authenticated = await Session.Instance.IsAuthenticated();
            _model.Loading = false;
            UserDialogs.Instance.HideLoading();
        }

        public async void ButtonClick_ByTitle(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new ByTitlePage(), true);
        }

        public async void ButtonClick_ByAuthor(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new ByAuthorPage(), true);
        }

        public async void ButtonClick_Login(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Comprobando credenciales");
            try
            {
                if (Int32.TryParse(_model.Username, out int iUsername))
                {
                    var result = await Session.Instance.Login(iUsername, _model.Password);
                    if (result)
                    {
                        _model.Authenticated = true;
                    }
                    else
                    {
                        _model.ErrorMessage = "Usuario o contraseña incorrectos";
                    }
                }
                else
                {
                    _model.ErrorMessage = "Usuario o contraseña incorrectos";
                }
            }
            catch
            {
                _model.ErrorMessage = "Audioteca no disponible";
            }
            UserDialogs.Instance.HideLoading();
        }
    }
}