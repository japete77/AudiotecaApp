using fonoteca.Helpers;
using RestSharp;

namespace fonoteca.Services
{
    public class ApiClient
    {
        private static ApiClient _instance;

        public static ApiClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ApiClient();
                }

                return _instance;
            }
        }

        public RestClient Client { get; }

        public ApiClient()
        {
            Client = new RestClient(AppSettings.Instance.FonotecaApiUrl);
            // Client.UseSerializer(() => new JsonNetSerializer());
        }
    }
}
