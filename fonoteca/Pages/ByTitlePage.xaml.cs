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
                _titles = await AudioLibrary.Instance.GetBooksByTitle(1, PAGE_SIZE);

                if (_titles == null)
                {
                    return;
                }

                var sorted = _titles.Titles
                                .OrderBy(o => o.Title)
                                .GroupBy(g => g.TitleSort)
                                .Select(s => new Grouping<string, TitleModel>(s.Key, s));

                // Clear the existing items before adding new ones
                _vm.Items.Clear();
                sorted.ToList().ForEach(item => _vm.Items.Add(item));

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

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        IEnumerable<Grouping<string, TitleModel>> sorted;        

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
        _vm.Items.Clear();
        sorted.ToList().ForEach(item => _vm.Items.Add(item));
    }
}