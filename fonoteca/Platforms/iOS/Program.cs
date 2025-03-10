using fonoteca.Helpers;
using ObjCRuntime;
using UIKit;

namespace fonoteca
{
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            AudioBookDataDir.StorageDirs = new List<StorageDir>
            {
                new StorageDir
                {
                    Name = "Memoria principal",
                    AbsolutePath = FileSystem.AppDataDirectory
                }
            };            

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
