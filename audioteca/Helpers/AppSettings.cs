using System;
using System.IO;
using Newtonsoft.Json;

namespace audioteca.Helpers
{
    public class AppSettings
    {
        private static AppSettings _instance;

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json"));
                }

                return _instance;
            }
        }

        public string AwsKey;
        public string AwsSecret;
        public string AwsPlatformApplicationArn;
        public string AwsTopicArn;
        public string FonotecaApiUrl;
    }
}
