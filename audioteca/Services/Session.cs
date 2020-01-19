using System.Threading.Tasks;
using audioteca.Exceptions;
using audioteca.Helpers;
using audioteca.Models.Api;
using audioteca.Models.Session;
using Newtonsoft.Json;
using RestSharp;
using Xamarin.Forms;

namespace audioteca.Services
{
    public class Session
    {
        private static Session _instance;
        private SessionInfo _sessionInfo = new SessionInfo();
        private string _lastError;
        private const string SESSION_INFO_KEY = "SessionInfo";

        public static Session Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new Session();
                }

                return _instance;
            }
        }

        public Session()
        {
            Application.Current.Properties.TryGetValue(SESSION_INFO_KEY, out object data);
            if (data != null) _sessionInfo = JsonConvert.DeserializeObject<SessionInfo>(data.ToString());
        }

        public async Task<bool> Login(int user, string password)
        {
            return await Task.Run<bool>(() =>
            {
                RestRequest request = new RestRequest("login", DataFormat.Json)
                {
                    Method = Method.POST
                };
                request.AddJsonBody(new LoginRequest { User = user, Password = password });

                var result = ApiClient.Instance.Client.Post<LoginResult>(request);
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException();
                }
                else if (result.ResponseStatus != ResponseStatus.Completed)
                {
                    throw new UnavailableException();
                }

                _sessionInfo = new SessionInfo
                {
                    Username = user,
                    Password = password,
                    Session = result.Data.Session
                };

                _lastError = result.Data.Message;

                if (result.Data.Success)
                {
                    Application.Current.Properties[SESSION_INFO_KEY] = JsonConvert.SerializeObject(_sessionInfo);
                    AsyncHelper.RunSync(() => Application.Current.SavePropertiesAsync());
                }

                return result.Data.Success;
            });
        }

        public async Task<bool> IsAuthenticated()
        {
            if (_sessionInfo == null) return false;

            if (!string.IsNullOrEmpty(_sessionInfo.Session))
            {
                // Check if session is still valid
                try
                {
                    await AudioLibrary.Instance.GetBooksByTitle(1, 1);
                    return true;
                }
                catch (UnauthorizedException)
                {
                    try
                    {
                        var result = await Login(_sessionInfo.Username, _sessionInfo.Password);
                        if (result)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                    }

                    return false;
                }
            }

            return false;
        }

        public string GetSession()
        {
            return _sessionInfo.Session;
        }

        public string GetLastError()
        {
            return _lastError;
        }
    }
}
