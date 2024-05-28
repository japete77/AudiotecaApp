using Android.App;
using Android.Runtime;
using fonoteca.Helpers;

namespace fonoteca
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
            AudioBookDataDir.StorageDirs = GetExternalFilesDirs(null)
                .Where(w => w.IsDirectory)
                .Select((s, index) => new StorageDir
                {
                    Name = $"Memoria {index} de {SizeHelper.SizeSuffix(s.TotalSpace)}",
                    AbsolutePath = s.AbsolutePath,
                    FreeSpace = s.FreeSpace,
                    TotalSpace = s.TotalSpace
                })
                .ToList();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
