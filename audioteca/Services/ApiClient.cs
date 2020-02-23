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
            Client = new RestClient("https://1dbbwygss8.execute-api.eu-west-1.amazonaws.com/Prod/api/v1/fonoteca/");
            Client.UseSerializer(() => new JsonNetSerializer());
        }
    }
}
