using fonoteca.Helpers;
using fonoteca.Services;
using Microsoft.UI.Xaml;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace fonoteca.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var dataPath = Path.Combine(currentPath, "data");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            // Get the drive where the current directory is located
            DriveInfo currentDrive = new DriveInfo(dataPath);

            // Get the name of the drive
            string driveName = currentDrive.Name;

            // Get the total size of the drive in bytes
            long totalSize = currentDrive.TotalSize;

            // Get the available free space on the drive in bytes
            long freeSpace = currentDrive.TotalFreeSpace;

            AudioBookDataDir.StorageDirs = new List<StorageDir>
            {
                new StorageDir
                {
                    AbsolutePath = dataPath,
                    Name = driveName,
                    FreeSpace = freeSpace,
                    TotalSpace = totalSize,
                }
            };

            Session.Instance.SetDataDir(dataPath);
            Session.Instance.SaveSession();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
