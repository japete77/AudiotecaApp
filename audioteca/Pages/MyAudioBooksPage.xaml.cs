using audioteca.Helpers;
using audioteca.Models.Audiobook;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace audioteca
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyAudioBooksPage : ContentPage
    {
        private readonly MyAudioBooksPageViewModel _model;
        private List<MyAudioBook> _myBooks;

        public MyAudioBooksPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _model = new MyAudioBooksPageViewModel();
            this.BindingContext = _model;
            _model.Loading = true;



            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (_model.Loading)
            {
                _myBooks = AudioBookStore.Instance.GetMyAudioBooks();

                if (_myBooks == null)
                {
                    return;
                }

                var sorted = _myBooks
                                .OrderBy(o => o.Book.Title)
                                .GroupBy(g => g.Book.TitleSort)
                                .Select(s => new Grouping<string, MyAudioBook>(s.Key, s));

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

            var bookId = (e.SelectedItem as MyAudioBook).Book.Id;

            if (!string.IsNullOrEmpty(bookId))
            {
                await Navigation.PushAsync(new AudioPlayerPage((e.SelectedItem as MyAudioBook).Book.Id), true);
            }
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ButtonClick_GoToPlayer(object sender, EventArgs e)
        {
            var currentBook = DaisyPlayer.Instance.GetDaisyBook();
            if (currentBook != null)
            {
                await Navigation.PushAsync(new AudioPlayerPage(currentBook.Id), true);
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            IEnumerable<Grouping<string, MyAudioBook>> sorted;

            listView.BeginRefresh();

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                sorted = _myBooks
                    .OrderBy(o => o.Book.Title)
                    .GroupBy(g => g.Book.TitleSort)
                    .Select(s => new Grouping<string, MyAudioBook>(s.Key, s));
            }
            else
            {
                sorted = _myBooks
                    .Where(s => TextHelper.RemoveDiacritics(s.Book.Title).ToUpper().Contains(TextHelper.RemoveDiacritics(e.NewTextValue).ToUpper()))
                    .OrderBy(o => o.Book.Title)
                    .GroupBy(g => g.Book.TitleSort)
                    .Select(s => new Grouping<string, MyAudioBook>(s.Key, s));
            }

            // Update items
            _model.Items.Clear();
            sorted.ToList().ForEach(item => _model.Items.Add(item));

            listView.EndRefresh();
        }

        //protected override void OnAppearing()
        //{
        //    _model.Items = new ObservableCollection<MyAudioBook>(AudioBookStore.Instance.GetMyAudioBooks());

        //    listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
        //    listView.BindingContext = _model.Items;

        //    _model.Loading = false;
        //}

        //public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    // has been set to null, do not 'process' tapped event
        //    if (e.SelectedItem == null) return;

        //    // de-select the row
        //    ((ListView)sender).SelectedItem = null;

        //    await Navigation.PushAsync(new AudioPlayerPage((e.SelectedItem as MyAudioBook).Book.Id), true);
        //}

        //private async void ButtonClick_Back(object sender, EventArgs e)
        //{
        //    await this.Navigation.PopAsync();
        //}
    }
}