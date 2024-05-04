using fonoteca.Helpers;
using fonoteca.Models.Audiobook;
using fonoteca.Services;
using fonoteca.ViewModels;
using System.Collections.ObjectModel;

namespace fonoteca.Pages;

public partial class MyAudioBooksPage : ContentPage
{
    private readonly MyAudioBooksPageViewModel _vm;
    private List<MyAudioBook> _myBooks;

    public MyAudioBooksPage(MyAudioBooksPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        vm.Items = new ObservableCollection<Grouping<string, MyAudioBook>>();
        _vm = vm;
    }

    protected override void OnAppearing()
    {
        _myBooks = AudioBookStore.Instance.GetMyAudioBooks();

        if (_myBooks == null)
        {
            return;
        }

        var sorted = _myBooks
            .Where(w => w.StatusKey == AudioBookStore.STATUS_COMPLETED)
            .OrderBy(o => o.Book.Title)
            .GroupBy(g => g.Book.TitleSort)
            .Select(s => new Grouping<string, MyAudioBook>(s.Key, s));

        _vm.Items.Clear();
        sorted.ToList().ForEach(item => _vm.Items.Add(item));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Reset selected items of the CollectionView
        ItemsCollectionView.SelectedItem = null;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        IEnumerable<Grouping<string, MyAudioBook>> sorted;

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
        _vm.Items.Clear();
        sorted.ToList().ForEach(item => _vm.Items.Add(item));
    }
}