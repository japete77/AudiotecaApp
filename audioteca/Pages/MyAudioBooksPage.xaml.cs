using Acr.UserDialogs;
using audioteca.Models.Audiobook;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyAudioBooksPage : ContentPage
    {
        private readonly MyAudioBooksPageViewModel _model;

        public MyAudioBooksPage()
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            _model = new MyAudioBooksPageViewModel();
            this.BindingContext = _model;
            _model.Loading = true;

            Title = "Mis audio libros";
            
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.Items = new ObservableCollection<MyAudioBook>(AudioBookStore.Instance.GetMyAudioBooks());
            
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.Items;
            
            _model.Loading = false;

            UserDialogs.Instance.HideLoading();
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            await Navigation.PushAsync(new AudioPlayerPage((e.SelectedItem as MyAudioBook).Book.Id), true);
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

    }
}