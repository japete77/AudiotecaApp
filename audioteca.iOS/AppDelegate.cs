using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Services;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace audioteca.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
              UIUserNotificationType.Alert |
              UIUserNotificationType.Badge |
              UIUserNotificationType.Sound,
              null
            );

            app.RegisterUserNotificationSettings(pushSettings);

            app.RegisterForRemoteNotifications();

            return base.FinishedLaunching(app, options);
        }

        public async override void RegisteredForRemoteNotifications(UIApplication application, NSData token)
        {
            var deviceToken = token.Description.Replace("<", "").Replace(">", "").Replace(" ", "");

            await NotificationsStore.Instance.RegisterUserNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            ShowAlert(application, "Error registrando notificaciones remotas", error.LocalizedDescription);
        }

        public async override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var aps = (userInfo["aps"] as NSDictionary);

            if (aps != null)
            {
                var alert = (aps["alert"] as NSString);
                var title = (aps["title"] as NSString);
                var date = (aps["date"] as NSString);
                var type = (aps["type"] as NSString);
                var contentId = (aps["id"] as NSString);

                await NotificationsStore.Instance.AddNotification(
                    new NotificationModel
                    {
                         Date = date != null ? date.Description : "",
                         Title = title != null ? title.Description : "",
                         Body = alert != null ? alert.Description : "",
                         Code = type != null ? type.Description : "",
                         ContentId = contentId != null ? contentId.Description : ""
                    }
                );

                await NotificationsStore.Instance.ShowNotification(0);
            }
        }

        private void ShowAlert(UIApplication application, string title, string message)
        {
            //Create Alert
            var errorAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            //Add Action
            errorAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            // Present Alert
            application.KeyWindow.RootViewController.PresentViewController(errorAlertController, true, null);
        }
    }
}
