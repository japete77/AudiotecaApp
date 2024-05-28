using CommunityToolkit.Mvvm.ComponentModel;

namespace fonoteca.ViewModels
{
    public partial class LoadingPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        public string message;
    }
}
