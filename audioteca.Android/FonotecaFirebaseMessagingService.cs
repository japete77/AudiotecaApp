using Android.App;
using Android.Content;
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
            await NotificationsStore.Instance.SaveDeviceToken(token);

            base.OnNewToken(token);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            if (message.Data.ContainsKey("notification"))
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Tienes una notificación nueva", message.Data["notification"], "Cerrar");
                });
            }

            base.OnMessageReceived(message);
        }
    }
}