using Firebase.CloudMessaging;
using Foundation;
using UIKit;

namespace fonoteca
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        // Usa [Export] para implementar el callback nativo y asignar el token APNs a Firebase.
        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            // Asigna el token APNs a Firebase Cloud Messaging
            Messaging.SharedInstance.ApnsToken = deviceToken;
            System.Diagnostics.Debug.WriteLine("Token APNs asignado a Firebase: " + deviceToken.ToString());
        }

        // (Opcional) Método para capturar errores en el registro de notificaciones.
        [Export("application:didFailToRegisterForRemoteNotificationsWithError:")]
        public void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            System.Diagnostics.Debug.WriteLine("Error al registrar notificaciones remotas: " + error.LocalizedDescription);
        }
    }
}
