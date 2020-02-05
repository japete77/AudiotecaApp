using Acr.UserDialogs;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ByAuthorPage : ContentPage
    {
        private const int PAGE_SIZE = 100;

        private readonly ByAuthorPageViewModel _model;

        public ByAuthorPage()
        {
            _model = new ByAuthorPageViewModel();
            this.BindingContext = _model;
            Title = "Por Autor";
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await LoadMore();
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var authorId = (e.SelectedItem as AuthorModel).Id;

            if (string.IsNullOrEmpty(authorId))
            {
                // Clicked fake (View more) item
                await LoadMore();
            }
            else
            {
                await Navigation.PushAsync(new ByAuthorTitlesPage((e.SelectedItem as AuthorModel).Id), true);
            }
        }

        private async Task LoadMore()
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            // Remove fake item (View more)
            if (_model.Items.Count > 0) _model.Items.RemoveAt(_model.Items.Count - 1);

            _model.Loading = false;

            var result = await AudioLibrary.Instance.GetAuthors(_model.Items.Count + 1, PAGE_SIZE);

            if (result != null && result.Authors != null)
            {
                result.Authors.ForEach(v => _model.Items.Add(v));
            }

            // Add fake item (View more) at the end of the list
            if (_model.Items.Count < result.Total)
            {
                _model.Items.Add(new AuthorModel { Name = "Ver mas autores" });
            }

            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.Items;

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;
        }

        public async void GoToHome_Click(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

    }
}
