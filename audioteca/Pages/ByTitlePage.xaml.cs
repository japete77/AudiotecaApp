using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using Xamarin.Forms;

namespace audioteca
{
    public partial class ByTitlePage : ContentPage, INotifyPropertyChanged
    {
        private const int PAGE_SIZE = 25;

        private ByTitlePageViewModel _model;

        public ByTitlePage()
        {
            UserDialogs.Instance.ShowLoading("Cargado");

            _model = new ByTitlePageViewModel();
            this.BindingContext = _model;
            Title = "Por Título";
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var result = AsyncHelper.RunSync<TitleResult>(() => AudioLibrary.Instance.GetBooksByTitle(_model.Items.Count + 1, PAGE_SIZE));

            if (result != null && result.Titles != null)
            {
                result.Titles.ForEach(v => _model.Items.Add(v));
            }
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.Items;

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;
        }

        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return; 

            this.Navigation.PushAsync(new BookDetails((e.SelectedItem as TitleModel).Id), true);

            // de-select the row
            ((ListView)sender).SelectedItem = null; 
        }

        public async void ButtonClick_LoadMore(object sender, EventArgs e)
        {
            await LoadMore();
        }

        private async Task LoadMore()
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            _model.Loading = false;

            var result = await AudioLibrary.Instance.GetBooksByTitle(_model.Items.Count + 1, PAGE_SIZE);

            if (result != null && result.Titles != null)
            {
                result.Titles.ForEach(v => _model.Items.Add(v));
            }
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.Items;

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;
        }
    }
}
