using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        private ConfigurationPageViewModel _model { get; }

        public ConfigurationPage()
        {
            _model = new ConfigurationPageViewModel();
            BindingContext = _model;
            _model.Items = new ObservableCollection<StorageDir>(AudioBookDataDir.StorageDirs);
            Title = "Configuración";
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.Speed = Session.Instance.GetSpeed();
            var storage = _model.Items
                .Where(w => w.AbsolutePath == Session.Instance.GetDataDir())
                .FirstOrDefault();
            _model.Storage = storage;
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }


        private void pickerStorage_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        

        public void ButtonClick_ClearCredentials(object sender, EventArgs e)
        {
            UserDialogs.Instance.Confirm(
                new ConfirmConfig
                {
                    Title = "Aviso",
                    Message = $"Esto limpiará las credenciales de usuario y tendrá que iniciar sesión de nuevo con un usuario y contraseña ¿desea continuar?",
                    OkText = "Si",
                    CancelText = "No",
                    OnAction = (action) =>
                    {
                        if (action)
                        {
                            Session.Instance.CleanCredentials();
                            Session.Instance.SaveSession();
                        }
                    }
                }
            );
        }

        private async void ButtonClick_SelectSpeeed(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new ConfigurationSpeedPage(), true);
        }

        private async void ButtonClick_SelectMemory(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new ConfigurationMemoryPage(), true);
        }
    }
}