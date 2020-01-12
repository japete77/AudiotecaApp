using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace audioteca
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            Title = "Inicio";
            InitializeComponent();
        }

        public void ButtonClick_MyAudiobooks(object sender, EventArgs e)
        {
        }

        public async void ButtonClick_Audiobooks(object sender, EventArgs e)
        {
            await this.Navigation.PushAsync(new AudioLibraryPage(), true);
        }

        public void ButtonClick_Notificacions(object sender, EventArgs e)
        {
        }

        public void ButtonClick_Configuration(object sender, EventArgs e)
        {
        }
    }
}
