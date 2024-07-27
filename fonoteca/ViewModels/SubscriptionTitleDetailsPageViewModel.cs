using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;
using fonoteca.Models.Audiobook;
using fonoteca.Pages;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class SubscriptionTitleDetailsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        string bookId;

        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        AudioBookDetailResult audioBook;

        [ObservableProperty]
        SubscriptionTitle title;

        [ObservableProperty]
        bool showDownload;

        [ObservableProperty]
        bool showCancel;

        [ObservableProperty]
        bool showPlay;

        [ObservableProperty]
        bool showStatus;

        [ObservableProperty]
        string statusDescription;

        [RelayCommand]
        public void Download()
        {
            AudioBookStore.Instance.OnProgress += Download_OnProgress;
            AudioBookStore.Instance.Download(AudioBook);
        }

        [RelayCommand]
        public void Cancel()
        {
            AudioBookStore.Instance.OnProgress -= Download_OnProgress;

            Download_OnProgress(
                new MyAudioBook
                {
                    StatusKey = AudioBookStore.STATUS_CANCELLED,
                    Book = new AudioBookDetailResult { Id = BookId }
                }
            );

            AudioBookStore.Instance.Cancel(AudioBook.Id);
        }

        [RelayCommand]
        public async Task Play()
        {
            await Shell.Current.Navigation.PushAsync(new AudioPlayerPage(new AudioPlayerPageViewModel { Id = AudioBook.Id }), true);
        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        public void Download_OnProgress(MyAudioBook abook)
        {
            if (abook != null && abook.Book.Id == BookId)
            {
                if (abook.StatusKey == AudioBookStore.STATUS_CANCELLED)
                {
                    ShowCancel = false;
                    ShowDownload = true;
                    ShowPlay = false;
                    ShowStatus = false;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_DOWNLOADING ||
                         abook.StatusKey == AudioBookStore.STATUS_PENDING)
                {
                    ShowCancel = true;
                    ShowDownload = false;
                    ShowPlay = false;
                    ShowStatus = true;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_DOWNLOADED ||
                         abook.StatusKey == AudioBookStore.STATUS_INSTALLING)
                {
                    ShowCancel = false;
                    ShowDownload = false;
                    ShowPlay = false;
                    ShowStatus = true;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_ERROR)
                {
                    ShowCancel = false;
                    ShowDownload = true;
                    ShowPlay = false;
                    ShowStatus = true;
                }
                else if (abook.StatusKey == AudioBookStore.STATUS_COMPLETED)
                {
                    ShowCancel = false;
                    ShowDownload = false;
                    ShowPlay = true;
                    ShowStatus = false;
                }

                StatusDescription = abook.StatusDescription;
            }
        }
    }
}
