using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using audioteca.Services;
using Xamarin.Forms;

namespace audioteca
{
    public partial class AudioLibraryPage : ContentPage, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        public bool Authenticated { get; set; } = false;
        public bool Loading { get; set; } = true;
        public string Username { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public AudioLibraryPage()
        {
            UserDialogs.Instance.ShowLoading("Verificando credenciales");

            this.BindingContext = this;
            Title = "Audioteca";
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Authenticated = Session.Instance.IsAuthenticated();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Authenticated)));
            Loading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
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

        public void ButtonClick_Login(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Comprobando credenciales");
            Task.Run(() =>
            {
                try
                {
                    int iUsername;
                    if (Int32.TryParse(Username, out iUsername))
                    {
                        if (Session.Instance.Login(iUsername, Password))
                        {
                            Authenticated = true;
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Authenticated)));
                        }
                        else
                        {
                            ErrorMessage = "Usuario o contraseña incorrectos";
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
                        }
                    }
                }
                catch
                {
                    ErrorMessage = "Audioteca no disponible";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
                }
                UserDialogs.Instance.HideLoading();
            });
        }
        
    }
}
