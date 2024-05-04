using fonoteca.Services;
using fonoteca.ViewModels;
using System.Net;

namespace fonoteca.Pages;

public partial class BookDetailsPage : ContentPage
{
    private BookDetailsPageViewModel _vm;
    public BookDetailsPage(BookDetailsPageViewModel vm)
	{
		InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        var book = AudioBookStore.Instance.GetMyAudioBook(vm.BookId);
        if (book != null)
        {
            _vm.Download_OnProgress(book);
            AudioBookStore.Instance.OnProgress += _vm.Download_OnProgress;
        }
        else
        {
            _vm.ShowCancel = false;
            _vm.ShowDownload = true;
            _vm.ShowPlay = false;
            _vm.ShowStatus = false;
        }
    }

    protected async override void OnAppearing()
    {
        _vm.AudioBook = await AudioLibrary.Instance.GetBookDetail(_vm.BookId.ToString());

        _vm.Loading = false;
    }

    
}