using System.Net.NetworkInformation;

namespace fonoteca.Services
{
    /// <summary>
    /// Static service to check the internet connection.
    /// The connection check is performed once on the first access and cached thereafter.
    /// </summary>
    public static class OfflineChecker
    {
        // Lazy initialization to perform the check only once and cache the result.
        private static readonly Lazy<bool> lazyIsConnected = new Lazy<bool>(() =>
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    // Ping the address 8.8.8.8 (Google DNS) with a timeout of 3000 ms
                    PingReply reply = ping.Send("8.8.8.8", 3000);
                    return (reply.Status == IPStatus.Success);
                }
            }
            catch (Exception)
            {
                // If an exception occurs, assume there's no internet connection.
                return false;
            }
        });

        /// <summary>
        /// Gets a value indicating whether there is an internet connection.
        /// The check is performed once on first access and the result is cached.
        /// </summary>
        public static bool IsConnected
        {
            get { return lazyIsConnected.Value; }
        }
    }
}
