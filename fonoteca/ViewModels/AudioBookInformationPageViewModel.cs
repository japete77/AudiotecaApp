using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Pages;

namespace fonoteca.ViewModels
{
    public partial class AudioBookInformationPageViewModel : ObservableObject
    {
        public AudioBookInformationPage _page;

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string creator;

        [ObservableProperty]
        private string date;

        [ObservableProperty]
        private string format;

        [ObservableProperty]
        private string identifier;

        [ObservableProperty]
        private string publisher;

        [ObservableProperty]
        private string subject;

        [ObservableProperty]
        private string source;

        [ObservableProperty]
        private string charset;

        [ObservableProperty]
        private string generator;

        [ObservableProperty]
        private string narrator;

        [ObservableProperty]
        private string producer;

        [ObservableProperty]
        private string totalTime;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        public async Task DeleteBook()
        {
            await _page.DeleteBook();
        }
    }
}
