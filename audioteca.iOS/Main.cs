using audioteca.Helpers;
using MediaManager;
using System;
using UIKit;

namespace audioteca.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // DataDir
            AudioBookDataDir.StorageDirs.Add(new StorageDir
            {
                Name = "Memoria principal",
                AbsolutePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            });

            CrossMediaManager.Current.Init();

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
