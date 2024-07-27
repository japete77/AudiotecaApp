using fonoteca.Services;
using fonoteca.ViewModels;

namespace fonoteca.Pages;

public partial class SubscriptionTitleDetailsPage : ContentPage
{
    private SubscriptionTitleDetailsPageViewModel _vm;
    private ILoadingService _loading;

    public SubscriptionTitleDetailsPage(SubscriptionTitleDetailsPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        _loading = Application.Current.Handler.MauiContext.Services.GetService<ILoadingService>();

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
}