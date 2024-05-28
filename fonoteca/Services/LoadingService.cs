using Mopups.Interfaces;
using Mopups.Services;
using fonoteca.Pages;

namespace fonoteca.Services
{
    public class LoadingService : ILoadingService, IDisposable
    {
        private readonly IPopupNavigation navigation;

        public LoadingService()
        {
            navigation = MopupService.Instance;
        }

        public async void Dispose()
        {
            await navigation.PopAsync();
        }

        public async Task<IDisposable> Show(string message = null)
        {
            await navigation.PushAsync(new LoadingPopupPage(new ViewModels.LoadingPopupViewModel { Message = message }), true);
            return this;
        }
    }
}
