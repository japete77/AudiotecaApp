using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fonoteca.Models.Api;
using fonoteca.Pages;
using fonoteca.Services;

namespace fonoteca.ViewModels
{
    public partial class NotificationDetailPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool loading;

        [ObservableProperty]
        private NotificationModel notification;

        [RelayCommand]
        public async Task GoToBack()
        {
            await Shell.Current.Navigation.PopAsync(true);
        }

        [RelayCommand]
        public async Task GoToPublishing()
        {
            if (Notification.Code == "CAT")
            {
                await AudioLibrary.Instance.RefreshBooks();
                await Shell.Current.Navigation.PushAsync(
                    new BookDetailsPage(
                        new BookDetailsPageViewModel
                        {
                            BookId = Notification.ContentId
                        }
                    ),
                    true
                );
            }
            else
            {
                var subscriptions = await AudioLibrary.Instance.GetSubscriptionTitles(Notification.Code);
                var subscription = subscriptions.Where(w => w.Id == Int32.Parse(Notification.ContentId)).FirstOrDefault();
                if (subscription != null)
                {
                    await Shell.Current.Navigation.PushAsync(
                        new SubscriptionTitleDetailsPage(
                            new SubscriptionTitleDetailsPageViewModel
                            {
                                BookId = $"{Notification.Code}{Notification.ContentId}",
                                Title = subscription,
                                AudioBook = new AudioBookDetailResult
                                {
                                    Id = $"{Notification.Code}{Notification.ContentId}",
                                    Title = subscription.Title,
                                }
                            }
                        ),
                        true
                    );
                }
            }
        }
    }
}
