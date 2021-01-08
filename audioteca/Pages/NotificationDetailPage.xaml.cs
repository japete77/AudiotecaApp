using audioteca.Models.Api;
using audioteca.Services;
using audioteca.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;

namespace audioteca
{
    public partial class NotificationDetailPage : ContentPage
    {
        private readonly NotificationDetailPageViewModel _model;

        private NotificationModel _notification;
        private int _index;

        public NotificationDetailPage(NotificationModel notification, int index)
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _index = index;
            _notification = notification;

            _model = new NotificationDetailPageViewModel
            {
                Loading = true,
                Notification = notification,
                ShowGoDetails = !string.IsNullOrEmpty(notification.ContentId) && notification.ContentId != "0"
            };

            this.BindingContext = _model;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _model.Loading = false;
        }

        private async void ButtonClick_Back(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }

        private async void ButtonClick_GoPublishing(object sender, EventArgs e)
        {
            _model.Loading = true;

            if (_notification.Code == "CAT")
            {
                await AudioLibrary.Instance.RefreshBooks();
                await this.Navigation.PushAsync(new BookDetails(_notification.ContentId));
            }
            else
            {
                var subscriptions = await AudioLibrary.Instance.GetSubscriptionTitles(_notification.Code);
                var subscription = subscriptions.Where(w => w.Id == Int32.Parse(_notification.ContentId)).FirstOrDefault();
                if (subscription != null)
                {
                    await this.Navigation.PushAsync(new SubscriptionTitleDetailsPage(subscription, _notification.Code));
                }
            }

            _model.Loading = false;
        }
    }
}
