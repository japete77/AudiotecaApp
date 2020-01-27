using Acr.UserDialogs;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace audioteca
{
    public partial class ByAuthorTitlesPage : ContentPage, INotifyPropertyChanged
    {
        private const int PAGE_SIZE = 25;

        private readonly string _authorId;

        private readonly ByTitlePageViewModel _model;

        public ByAuthorTitlesPage(string id)
        {
            _model = new ByTitlePageViewModel();
            this.BindingContext = _model;
            Title = "Títulos";
            _authorId = id;
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

            var bookId = (e.SelectedItem as TitleModel).Id;

            if (string.IsNullOrEmpty(bookId))
            {
                // Clicked fake (View more) item
                await LoadMore();
            }
            else
            {
                await Navigation.PushAsync(new BookDetails((e.SelectedItem as TitleModel).Id), true);
            }
        }

        private async Task LoadMore()
        {
            UserDialogs.Instance.ShowLoading("Cargando");

            // Remove fake item (View more)
            if (_model.Items.Count > 0) _model.Items.RemoveAt(_model.Items.Count - 1);

            _model.Loading = false;

            var result = await AudioLibrary.Instance.GetBooksByAuthor(_authorId, _model.Items.Count + 1, PAGE_SIZE);

            if (result != null && result.Titles != null)
            {
                result.Titles.ForEach(v => _model.Items.Add(v));
            }

            // Add fake item (View more) at the end of the list
            if (_model.Items.Count < result.Total)
            {
                _model.Items.Add(new TitleModel { Title = "Ver mas títulos" });
            }

            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
            listView.BindingContext = _model.Items;

            UserDialogs.Instance.HideLoading();

            _model.Loading = false;
        }
    }
}
