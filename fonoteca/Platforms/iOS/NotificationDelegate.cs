using UIKit;
using UserNotifications;

public class NotificationDelegate : UNUserNotificationCenterDelegate
{
    // Se invoca cuando la app está en primer plano y recibe una notificación
    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        // Indica cómo se mostrará la notificación (alerta, sonido, badge)
        if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
        {
            completionHandler(UNNotificationPresentationOptions.List | UNNotificationPresentationOptions.Banner | UNNotificationPresentationOptions.Sound);
        }
        else
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }
    }

    // Se invoca cuando el usuario interactúa con una notificación (por ejemplo, al tocarla)
    public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
    {
        // Aquí puedes extraer datos de la notificación y redirigir al usuario a la sección correspondiente de la app
        var userInfo = response.Notification.Request.Content.UserInfo;
        // Procesa 'userInfo' según tus necesidades

        completionHandler();
    }
}
