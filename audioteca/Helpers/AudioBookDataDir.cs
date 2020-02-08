using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace audioteca.Helpers
{
    public static class AudioBookDataDir
    {
        public static List<StorageDir> StorageDirs { get; set; }

        public static string GetAndroidExternalSdCardPath()
        {
            var procMounts = System.IO.File.ReadAllText("/proc/mounts");
            var candidateProcMountEntries = new List<string>();
            candidateProcMountEntries.AddRange(procMounts.Split('\n', '\r'));
            candidateProcMountEntries.RemoveAll(s => s.IndexOf("storage", StringComparison.OrdinalIgnoreCase) < 0);
            var bestCandidate = candidateProcMountEntries
              .FirstOrDefault(s => s.IndexOf("ext", StringComparison.OrdinalIgnoreCase) >= 0
                                   && s.IndexOf("sd", StringComparison.OrdinalIgnoreCase) >= 0
                                   && s.IndexOf("vfat", StringComparison.OrdinalIgnoreCase) >= 0);

            // e.g. /dev/block/vold/179:9 /storage/extSdCard vfat rw,dirsync,nosuid, blah
            if (!string.IsNullOrWhiteSpace(bestCandidate))
            {
                var sdCardEntries = bestCandidate.Split(' ');
                var sdCardEntry = sdCardEntries.FirstOrDefault(s => s.IndexOf("/storage/", System.StringComparison.OrdinalIgnoreCase) >= 0);
                return !string.IsNullOrWhiteSpace(sdCardEntry) ? string.Format("{0}", sdCardEntry) : string.Empty;
            }

            return string.Empty;
        }
    }

    public class StorageDir
    {
        public string Name { get; set; }
        public string AbsolutePath { get; set; }
        public long FreeSpace { get; set; }
        public long TotalSpace { get; set; }
    }

    public class SizeHelper
    {
        static readonly string[] SizeSuffixes =
                       { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
}
