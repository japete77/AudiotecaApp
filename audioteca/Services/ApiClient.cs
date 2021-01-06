using System;
using audioteca.Helpers;
using RestSharp;

namespace audioteca.Services
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
            Client.UseSerializer(() => new JsonNetSerializer());
        }
    }
}
