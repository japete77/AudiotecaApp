using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;

namespace fonoteca.ViewModels
{
    public partial class BookDetailsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        string bookId;

        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        AudioBookDetailResult audioBook;

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
        public async Task Download()
        {
            await Task.CompletedTask;
        }

        [RelayCommand]
        public async Task Cancel()
        {
            await Task.CompletedTask;
        }

        [RelayCommand]
        public async Task Play()
        {
            await Task.CompletedTask;
        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
