using Android.App;
using Android.Content;
using audioteca.Models.Api;
using audioteca.Services;
using Firebase.Messaging;

namespace audioteca.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FonotecaFirebaseMessagingService : FirebaseMessagingService
    {
        public async override void OnNewToken(string token)
        {
            await NotificationsStore.Instance.RegisterUserNotifications(token);

            base.OnNewToken(token);
        }

        public async override void OnMessageReceived(RemoteMessage message)
        {
            if (message.Data.Count > 0)
            {
                await NotificationsStore.Instance.AddNotification(
                    new NotificationModel
                    {
                        Title = message.Data.ContainsKey("title") ? message.Data["title"] : "",
                        Body = message.Data.ContainsKey("message") ? message.Data["message"] : "",
                        Code = message.Data.ContainsKey("type") ? message.Data["type"] : "",
                        ContentId = message.Data.ContainsKey("id") ? message.Data["id"] : "",
                        Date = message.Data.ContainsKey("date") ? message.Data["date"] : "",
                    }
                );

                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await NotificationsStore.Instance.ShowNotification(0);
                });
            }

            base.OnMessageReceived(message);
        }
    }
}