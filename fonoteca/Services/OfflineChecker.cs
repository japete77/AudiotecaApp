using System;
using System.Net.Http;

namespace fonoteca.Services
{
    /// <summary>
    /// Servicio estático para comprobar la conexión a internet.
    /// La comprobación se realiza una sola vez en el primer acceso y se almacena el resultado en caché.
    /// </summary>
    public static class OfflineChecker
    {
        // Cachea la primera comprobación; si queréis refrescar, exponed un método Reset().
        private static readonly Lazy<Task<bool>> lazyIsConnected =
            new(() => ProbeAsync(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>Resultado cacheado de la primera comprobación.</summary>
        public static Task<bool> IsConnectedAsync => lazyIsConnected.Value;

        /// <summary>Comprobación rápida sin HTTP.</summary>
        public static bool HasInternetAccessFlag =>
            Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        /// <summary>
        /// Sonda real a Internet (HEAD a un 204) con timeout corto.
        /// Solo se llama la primera vez por el Lazy, y solo si el flag dice que hay Internet.
        /// </summary>
        private static async Task<bool> ProbeAsync()
        {
            if (!HasInternetAccessFlag)
                return false;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                using var client = new HttpClient();
                using var req = new HttpRequestMessage(HttpMethod.Head, "https://connectivitycheck.gstatic.com/generate_204");
                var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                                       .ConfigureAwait(false);
                // 204/200 etc. => consideramos “online”
                return (int)resp.StatusCode < 500;
            }
            catch
            {
                return false;
            }
        }
    }
}
