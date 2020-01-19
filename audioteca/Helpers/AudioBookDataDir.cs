using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace audioteca.Helpers
{
    public static class AudioBookDataDir
    {
        public static string DataDir;

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
}
