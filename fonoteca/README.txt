Notificaciones

MAUI+iOS: https://cedricgabrang.medium.com/firebase-push-notifications-in-net-maui-ios-2f4388bf1ac
MAUI+Android: https://cedricgabrang.medium.com/firebase-push-notifications-in-net-maui-android-32c808844d7e
Implementación Notificaciones MAUI+Android: https://www.youtube.com/watch?v=KyCNS9t_J-E


How to configure Android Notifications
- Go to Firebase Console
- Create a new project
- Add a new Android app
- Download google-services.json and add it to your project

AWS SNS Configuration
- Go to AWS Console
- Create a new SNS Application Platform
- Set the Application Name
- Select the platform Firebase Cloud Messaging (FCM)
- Add the Server Key from Firebase Console. 
  JSON file can be generated from Firebase Console from Project -> Service Account -> Generate New Private Key
- Create a new topic

Add Firebase Plugin in App (Android)
- Follow https://github.com/Apodaca09/Push-Notifications
- Take into account that libraries installation should be done using command line and after enabling long paths
in Windows: https://support.safe.com/hc/en-us/articles/25407469875469-Windows-Enable-long-file-path-support
	dotnet add package <package-name> --version x.y.z