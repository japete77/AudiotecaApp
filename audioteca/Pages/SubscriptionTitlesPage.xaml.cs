using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Acr.UserDialogs;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using Xamarin.Forms;

namespace audioteca
{
    public partial class SubscriptionTitlesPage : ContentPage
    {
        private readonly SubscriptionTitlesPageViewModel _model;

        private List<SubscriptionTitle> _titles;
        private string _code;

        public SubscriptionTitlesPage(string code)
        {
            _code = code;

            NavigationPage.SetHasNavigationBar(this, false);

            _model = new SubscriptionTitlesPageViewModel
            {
                Loading = true,
                Items = new ObservableCollection<SubscriptionTitle>()
            };

            this.BindingContext = _model;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (_model.Loading)
            {
                UserDialogs.Instance.ShowLoading("Cargando suscripciones");

                try
                {
                    _titles = await AudioLibrary.Instance.GetSubscriptionTitles(_code);

                    if (_titles == null)
                    {
                        return;
                    }

                    // bind model
                    listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
                    listView.BindingContext = _model.Items;
                    listView.BeginRefresh();

                    _titles.ForEach(item => _model.Items.Add(item));

                    listView.EndRefresh();
                }
                catch
                {

                }

                _model.Loading = false;

                UserDialogs.Instance.HideLoading();
            }
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            await Navigation.PushAsync(new SubscriptionTitleDetailsPage(e.SelectedItem as SubscriptionTitle, _code), true);
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}
