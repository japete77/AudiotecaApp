using fonoteca.Helpers;
using fonoteca.Models.Api;
using fonoteca.Services;
using fonoteca.ViewModels;
using System.Collections.ObjectModel;

namespace fonoteca.Pages;

public partial class ByAuthorPage : ContentPage
{
    private const int PAGE_SIZE = int.MaxValue;
    private readonly ByAuthorPageViewModel _vm;
    private AuthorsResult _authors;

    public ByAuthorPage(ByAuthorPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        vm.Items = new ObservableCollection<Grouping<string, AuthorModel>>();
        vm.Loading = true;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        if (_vm.Loading)
        {
            _authors = await AudioLibrary.Instance.GetAuthors(1, PAGE_SIZE);

            if (_authors == null)
            {
                return;
            }

            var sorted = _authors.Authors
                            .OrderBy(o => o.Name)
                            .GroupBy(g => g.NameSort)
                            .Select(s => new Grouping<string, AuthorModel>(s.Key, s));

            // Clear the existing items before adding new ones
            _vm.Items.Clear();
            sorted.ToList().ForEach(item => _vm.Items.Add(item));

            _vm.Loading = false;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Reset selected items of the CollectionView
        ItemsCollectionView.SelectedItem = null;
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        IEnumerable<Grouping<string, AuthorModel>> sorted;

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
        _vm.Items.Clear();
        sorted.ToList().ForEach(item => _vm.Items.Add(item));
    }
}