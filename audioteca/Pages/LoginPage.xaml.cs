using Acr.UserDialogs;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly LoginPageViewModel _model;
        private const string DefaultPassword = "1234";

        public LoginPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new LoginPageViewModel();
            BindingContext = _model;

            InitializeComponent();
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
                        // Warm up audio books
                        await AudioLibrary.Instance.WarmUp();

                        if (_model.Password == DefaultPassword)
                        {
                            // Go to ChangePassword page
                            UserDialogs.Instance.HideLoading();

                            var currentPage = Navigation.NavigationStack.Last();
                            await Navigation.PushAsync(new ChangePasswordPage(), true);
                            Navigation.RemovePage(currentPage);
                        }
                        else
                        {
                            // Go to Main page
                            UserDialogs.Instance.HideLoading();

                            var currentPage = Navigation.NavigationStack.Last();
                            await Navigation.PushAsync(new MainPage(), true);
                            Navigation.RemovePage(currentPage);
                        }

                        await NotificationsStore.Instance.RegisterUserNotifications();

                        await NotificationsStore.Instance.RefreshNotifications();
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
    }
}
