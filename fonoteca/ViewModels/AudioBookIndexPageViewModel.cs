using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Daisy;

namespace fonoteca.ViewModels
{
    public partial class AudioBookIndexPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<SmilInfo> items;

        [ObservableProperty]
        private bool loading;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }
    }
}
