using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Pages;
namespace fonoteca.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string versionInfo;

        [RelayCommand]
        async Task GoToMyAudioBooks()
        {
            await Shell.Current.GoToAsync(nameof(MyAudioBooksPage));
        }

        [RelayCommand]
        async Task GoToAudioBooks()
        {
            await Shell.Current.GoToAsync(nameof(AudioLibraryPage));
        }

        [RelayCommand]
        async Task GoToSubscriptions()
        {
            // await Shell.Current.GoToAsync(nameof());
            await Task.CompletedTask;
        }

        [RelayCommand]
        async Task GoToNotifications()
        {
            // await Shell.Current.GoToAsync(nameof());
            await Task.CompletedTask;
        }

        [RelayCommand]
        async Task GoToConfiguration()
        {
            await Shell.Current.GoToAsync(nameof(ConfigurationPage));
        }
    }
}
