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
        public AudioLibraryPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

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

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}
