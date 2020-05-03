using System;
using System.Linq;
using Acr.UserDialogs;
using audioteca.Services;
using audioteca.ViewModels;
using Xamarin.Forms;

namespace audioteca
{
    public partial class ChangePasswordPage : ContentPage
    {
        private readonly ChangePasswordPageViewModel _model;
        private const string DefaultPassword = "1234";

        public ChangePasswordPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new ChangePasswordPageViewModel();
            BindingContext = _model;

            InitializeComponent();
        }

        public async void ButtonClick_ChangePassword(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_model.NewPassword) &&
                _model.NewPassword != DefaultPassword &&
                _model.NewPassword == _model.ConfirmNewPassword)
            {
                try
                {
                    UserDialogs.Instance.ShowLoading("Cambiando contraseña");

                    await Session.Instance.ChangePassword(_model.NewPassword);
                    Session.Instance.SetPassword(_model.NewPassword);
                    Session.Instance.SaveSession();

                    var currentPage = Navigation.NavigationStack.Last();
                    await Navigation.PushAsync(new MainPage(), true);
                    Navigation.RemovePage(currentPage);
                }
                finally
                {
                    UserDialogs.Instance.HideLoading();
                }
            }
            else
            {
                _model.ErrorMessage = "Las contraseñas no coinciden o no son válidas";
            }
        }
    }
}
