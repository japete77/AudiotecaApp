using fonoteca.Helpers;
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
        // Inicialización diferida para realizar la comprobación solo una vez y almacenar el resultado.
        private static readonly Lazy<bool> lazyIsConnected = new Lazy<bool>(() =>
        {
            try
            {
                // Obtener la URL base desde AppSettings.
                string baseUrl = AppSettings.Instance.FonotecaApiUrl;
                // Construir la URL completa añadiendo "firebase/android".
                string fullUrl = new Uri(new Uri(baseUrl), "firebase/android").ToString();

                using (HttpClient client = new HttpClient())
                {
                    // Configurar el timeout a 3 segundos.
                    client.Timeout = TimeSpan.FromSeconds(3);

                    // Realizar la solicitud GET de forma sincrónica.
                    HttpResponseMessage response = client.GetAsync(fullUrl).GetAwaiter().GetResult();

                    // Retornar true si el código de estado indica éxito.
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                // Si ocurre alguna excepción, asumimos que no hay conexión a internet.
                return false;
            }
        });

        /// <summary>
        /// Indica si hay conexión a internet.
        /// La comprobación se realiza una sola vez en el primer acceso y el resultado se almacena en caché.
        /// </summary>
        public static bool IsConnected
        {
            get { return lazyIsConnected.Value; }
        }
    }
}
