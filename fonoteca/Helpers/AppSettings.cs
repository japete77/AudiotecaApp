using Newtonsoft.Json;
using System.Reflection;

namespace fonoteca.Helpers
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
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = "fonoteca.appsettings.json";

                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd();
                        _instance = JsonConvert.DeserializeObject<AppSettings>(result);
                    }
                }

                return _instance;
            }
        }

        public string AwsKey;
        public string AwsSecret;
        public string AwsPlatformApplicationArnAndroid;
        public string AwsPlatformApplicationArnIOS;
        public string AwsTopicArn;
        public string AwsTopicName;
        public string FonotecaApiUrl;
        public string VersionInfo;
    }
}
