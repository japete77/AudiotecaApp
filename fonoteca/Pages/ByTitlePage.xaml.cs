using fonoteca.Helpers;
using fonoteca.Models.Api;
using fonoteca.Services;
using fonoteca.ViewModels;
using System.Collections.ObjectModel;

namespace fonoteca.Pages;

public partial class ByTitlePage : ContentPage
{
    private const int PAGE_SIZE = int.MaxValue;
    private readonly ByTitlePageViewModel _vm;
    private TitleResult _titles;
    private ILoadingService _loading;

    public ByTitlePage(ByTitlePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        vm.Items = new ObservableCollection<Grouping<string, TitleModel>>();
        vm.Loading = true;
        _vm = vm;        
        _loading = Application.Current.Handler.MauiContext.Services.GetService<ILoadingService>();
    }

    protected override async void OnAppearing()
    {
        if (_vm.Loading)
        {
            using (await _loading.Show("Cargando"))
            {
                _titles = await AudioLibrary.Instance.GetBooksByTitle(1, PAGE_SIZE, _vm.SortByRecent);

                if (_titles == null)
                {
                    return;
                }

                ShowTitles(string.Empty);

                _vm.Loading = false;
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Reset selected items of the CollectionView
        ItemsCollectionView.SelectedItem = null;
    }

    private void ShowTitles(string filterText)
    {
        IEnumerable<Grouping<string, TitleModel>> sorted;        

        if (string.IsNullOrEmpty(filterText))
        {
            if (_vm.SortByRecent)
            {
                sorted = _titles.Titles
                            .GroupBy(g => string.Empty)
                            .Select(s => new Grouping<string, TitleModel>(s.Key, s));
            }
            else
            {
                sorted = _titles.Titles
                            .OrderBy(o => o.Title)
                            .GroupBy(g => g.TitleSort)
                            .Select(s => new Grouping<string, TitleModel>(s.Key, s));
            }
        }
        else
        {
            if (_vm.SortByRecent)
            {
                sorted = _titles.Titles
                    .Where(s => TextHelper.RemoveDiacritics(s.Title).ToUpper().Contains(TextHelper.RemoveDiacritics(filterText).ToUpper()))                    
                    .GroupBy(g => string.Empty)
                    .Select(s => new Grouping<string, TitleModel>(s.Key, s));
            }
            else
            {
                sorted = _titles.Titles
                    .Where(s => TextHelper.RemoveDiacritics(s.Title).ToUpper().Contains(TextHelper.RemoveDiacritics(filterText).ToUpper()))
                    .OrderBy(o => o.Title)
                    .GroupBy(g => g.TitleSort)
                    .Select(s => new Grouping<string, TitleModel>(s.Key, s));
            }
        }

        // Update items
        var newItems = new ObservableCollection<Grouping<string, TitleModel>>(sorted);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _vm.Items = newItems;
        });
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        ShowTitles(e.NewTextValue);
    }
}