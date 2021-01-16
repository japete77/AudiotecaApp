using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ByTitlePage : ContentPage
    {
        private const int PAGE_SIZE = Int32.MaxValue;

        private readonly ByTitlePageViewModel _model;

        private TitleResult _titles;

        public ByTitlePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new ByTitlePageViewModel
            {
                Loading = true,
                Items = new ObservableCollection<Grouping<string, TitleModel>>()
            };

            this.BindingContext = _model;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (_model.Loading)
            {
                _titles = await AudioLibrary.Instance.GetBooksByTitle(1, PAGE_SIZE);

                if (_titles == null)
                {
                    return;
                }

                var sorted = _titles.Titles
                                .OrderBy(o => o.Title)
                                .GroupBy(g => g.TitleSort)
                                .Select(s => new Grouping<string, TitleModel>(s.Key, s));

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
            _model.Loading = true;

            // has been set to null, do not 'process' tapped event
            if (e.SelectedItem == null) return;

            // de-select the row
            ((ListView)sender).SelectedItem = null;

            var bookId = (e.SelectedItem as TitleModel).Id;

            if (!string.IsNullOrEmpty(bookId))
            {
                await Navigation.PushAsync(new BookDetails((e.SelectedItem as TitleModel).Id), true);
            }

            _model.Loading = false;
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<Grouping<string, TitleModel>> sorted;

            listView.BeginRefresh();

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                sorted = _titles.Titles
                    .OrderBy(o => o.Title)
                    .GroupBy(g => g.TitleSort)
                    .Select(s => new Grouping<string, TitleModel>(s.Key, s));
            }
            else
            {
                sorted = _titles.Titles
                    .Where(s => TextHelper.RemoveDiacritics(s.Title).ToUpper().Contains(TextHelper.RemoveDiacritics(e.NewTextValue).ToUpper()))
                    .OrderBy(o => o.Title)
                    .GroupBy(g => g.TitleSort)
                    .Select(s => new Grouping<string, TitleModel>(s.Key, s));
            }

            // Update items
            _model.Items.Clear();
            sorted.ToList().ForEach(item => _model.Items.Add(item));

            listView.EndRefresh();
        }
    }
}
