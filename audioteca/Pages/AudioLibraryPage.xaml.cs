using Acr.UserDialogs;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AudioLibraryPage : ContentPage
    {
        private readonly AudioLibraryPageViewModel _model;

        private const string DefaultPassword = "1234";

        public AudioLibraryPage()
        {
            UserDialogs.Instance.ShowLoading("Verificando credenciales");

            _model = new AudioLibraryPageViewModel();
            this.BindingContext = _model;
            _model.Loading = true;
            Title = "Fonoteca";
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            _model.Authenticated = await Session.Instance.IsAuthenticated();
            _model.MustChangePassword = _model.Authenticated && Session.Instance.GetPassword() == DefaultPassword;
            _model.IsAccesible = _model.Authenticated && !_model.MustChangePassword;
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

                        if (_model.Password == DefaultPassword)
                        {
                            _model.MustChangePassword = true;
                        }
                        else
                        {
                            _model.MustChangePassword = false;
                        }

                        _model.IsAccesible = _model.Authenticated && !_model.MustChangePassword;
                        _model.ErrorMessage = "";
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
                _model.ErrorMessage = "Fonoteca no disponible";
            }

            UserDialogs.Instance.HideLoading();
        }

        public async void ButtonClick_ChangePassword(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_model.NewPassword) &&
                _model.NewPassword != DefaultPassword && 
                _model.NewPassword == _model.ReNewPassword)
            {
                try
                {
                    UserDialogs.Instance.ShowLoading("Cambiando contraseña");

                    await Session.Instance.ChangePassword(_model.NewPassword);
                    _model.IsAccesible = true;
                    _model.Authenticated = true;
                    _model.MustChangePassword = false;
                    Session.Instance.SetPassword(_model.NewPassword);
                    Session.Instance.SaveSession();
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


        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
