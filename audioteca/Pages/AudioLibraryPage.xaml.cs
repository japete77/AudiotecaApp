using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace audioteca
{
    public partial class AudioLibraryPage : ContentPage, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        public bool Authenticated { get; set; } = false;
        public string Username { get; set; }
        public string Password { get; set; }

        public AudioLibraryPage()
        {
            this.BindingContext = this;
            Title = "Audioteca";
            Authenticated = false;
            InitializeComponent();
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
                Thread.Sleep(3000);
                UserDialogs.Instance.HideLoading();
            });
            Authenticated = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Authenticated)));
        }
        
    }
}
