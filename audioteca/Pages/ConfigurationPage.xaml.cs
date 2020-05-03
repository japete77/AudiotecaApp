using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new ConfigurationPageViewModel();
            BindingContext = _model;
            _model.Items = new ObservableCollection<StorageDir>(AudioBookDataDir.StorageDirs);
            _model.HasExternalMemory = _model.Items.Count > 1;

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

        public void ButtonClick_ClearCredentials(object sender, EventArgs e)
        {
            UserDialogs.Instance.Confirm(
                new ConfirmConfig
                {
                    Title = "Aviso",
                    Message = $"Esto limpiará las credenciales de usuario y tendrá que iniciar sesión de nuevo con un usuario y contraseña ¿desea continuar?",
                    OkText = "Si",
                    CancelText = "No",
                    OnAction = async (action) =>
                    {
                        if (action)
                        {
                            Session.Instance.CleanCredentials();
                            Session.Instance.SaveSession();

                            // Navigate to login page and clean up stack 
                            var pages = Navigation.NavigationStack.ToList();
                            await Navigation.PushAsync(new LoginPage(), true);
                            pages.ForEach(item => Navigation.RemovePage(item));
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

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}