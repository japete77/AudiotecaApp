using Acr.UserDialogs;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Title = "Configuración";
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.Speed = Session.Instance.GetSpeed();
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            DaisyPlayer.Instance.SetSpeed((float)_model.Speed);
            Session.Instance.SetSpeed(_model.Speed);
            Session.Instance.SaveSession();
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

    }
}