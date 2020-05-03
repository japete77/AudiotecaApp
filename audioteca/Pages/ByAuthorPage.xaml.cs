using Acr.UserDialogs;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ByAuthorPage : ContentPage
    {
        private const int PAGE_SIZE = Int32.MaxValue;

        private readonly ByAuthorPageViewModel _model;

        private AuthorsResult _authors;

        public ByAuthorPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new ByAuthorPageViewModel()
            {
                Loading = true,
                Items = new ObservableCollection<Grouping<string, AuthorModel>>()
            };

            this.BindingContext = _model;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (_model.Loading)
            {
                _authors = await AudioLibrary.Instance.GetAuthors(1, PAGE_SIZE);

                if (_authors == null)
                {
                    UserDialogs.Instance.HideLoading();
                    return;
                }

                var sorted = _authors.Authors
                                .OrderBy(o => o.Name)
                                .GroupBy(g => g.NameSort)
                                .Select(s => new Grouping<string, AuthorModel>(s.Key, s));

                //create a new collection of groups
                listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
                listView.BindingContext = _model.Items;
                listView.IsGroupingEnabled = true;
                listView.GroupDisplayBinding = new Binding("Key");
                listView.GroupShortNameBinding = new Binding("Key");

                listView.BeginRefresh();

                sorted.ToList().ForEach(item => _model.Items.Add(item));

                listView.EndRefresh();

                _model.Loading = false;
            }
        }

        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var authorId = (e.SelectedItem as AuthorModel).Id;

            if (!string.IsNullOrEmpty(authorId))
            {
                await Navigation.PushAsync(new ByAuthorTitlesPage((e.SelectedItem as AuthorModel).Id), true);
            }
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<Grouping<string, AuthorModel>> sorted;

            listView.BeginRefresh();

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                sorted = _authors.Authors
                    .OrderBy(o => o.Name)
                    .GroupBy(g => g.NameSort)
                    .Select(s => new Grouping<string, AuthorModel>(s.Key, s));
            }
            else
            {
                sorted = _authors.Authors
                    .Where(s => TextHelper.RemoveDiacritics(s.Name).ToUpper().Contains(TextHelper.RemoveDiacritics(e.NewTextValue).ToUpper()))
                    .OrderBy(o => o.Name)
                    .GroupBy(g => g.NameSort)
                    .Select(s => new Grouping<string, AuthorModel>(s.Key, s));
            }

            // Update items
            _model.Items.Clear();
            sorted.ToList().ForEach(item => _model.Items.Add(item));

            listView.EndRefresh();
        }
    }
}
