using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using Xamarin.Forms;

namespace audioteca
{
    public partial class ByTitlePage : ContentPage, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<TitleModel> items = new ObservableCollection<TitleModel>();
        public bool Loading { get; set; } = true;
        private const int PAGE_SIZE = 25;

        public ByTitlePage()
        {
            UserDialogs.Instance.ShowLoading("Cargado");

            this.BindingContext = this;
            Title = "Por Título";
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var result = AsyncHelper.RunSync<TitleResult>(() => AudioLibrary.Instance.GetBooksByTitle(items.Count + 1, PAGE_SIZE));

            if (result != null && result.Titles != null)
            {
                result.Titles.ForEach(v => items.Add(v));
            }
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = items;

            UserDialogs.Instance.HideLoading();
            Loading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return; // has been set to null, do not 'process' tapped event
            this.Navigation.PushAsync(new BookDetails((e.SelectedItem as TitleModel).Id), true);
            //DisplayAlert("Tapped", e.SelectedItem + " row was tapped", "OK");
            ((ListView)sender).SelectedItem = null; // de-select the row
        }

        public async void ButtonClick_LoadMore(object sender, EventArgs e)
        {
            await LoadMore();
        }

        private async Task LoadMore()
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            Loading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));

            var result = await AudioLibrary.Instance.GetBooksByTitle(items.Count + 1, PAGE_SIZE);

            if (result != null && result.Titles != null)
            {
                result.Titles.ForEach(v => items.Add(v));
            }
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = items;

            UserDialogs.Instance.HideLoading();

            Loading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
        }
    }
}
