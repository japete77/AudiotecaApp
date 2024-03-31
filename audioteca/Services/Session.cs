using System;
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
        private readonly SessionInfo _sessionInfo = new SessionInfo();
        private string _lastError;
        private const string SESSION_INFO_KEY = "SessionInfo";

        private static Session _instance;
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

                _sessionInfo.Username = user;
                _sessionInfo.Password = password;
                _sessionInfo.Session = result.Data.Session;
                _sessionInfo.Speed = 1.0f;

                _lastError = result.Data.Message;

                if (result.Data.Success)
                {
                    SaveSession();
                }

                return result.Data.Success;
            });
        }

        public async Task ChangePassword(string newPassword)
        {
            await Task.Run(() =>
            {
                RestRequest request = new RestRequest("change-password", DataFormat.Json)
                {
                    Method = Method.POST
                };
                request.AddJsonBody(new ChangePasswordRequest { Session = _sessionInfo.Session, NewPassword = newPassword });

                var result = ApiClient.Instance.Client.Post<ChangePasswordRequest>(request);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("No se ha podido cambiar la contraseña");
                }
            });
        }

        public async Task<bool> IsAuthenticated()
        {
            if (_sessionInfo == null ||
                _sessionInfo.Username == 0 ||
                _sessionInfo.Password == null) return false;

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

            CleanCredentials();

            return false;
        }

        public string GetSession()
        {
            return _sessionInfo.Session;
        }

        public string GetPassword()
        {
            return _sessionInfo.Password;
        }

        public string GetLastError()
        {
            return _lastError;
        }

        public double GetSpeed()
        {
            return _sessionInfo.Speed;
        }

        public string GetDataDir()
        {
            return _sessionInfo.DataDir;
        }

        public void SetSpeed(double speed)
        {
            _sessionInfo.Speed = speed;
        }

        public void SetPassword(string password)
        {
            _sessionInfo.Password = password;
        }

        public void CleanCredentials()
        {
            _sessionInfo.Username = 0;
            _sessionInfo.Password = null;
            _sessionInfo.Session = null;
        }

        public void SetDataDir(string dataDir)
        {
            _sessionInfo.DataDir = dataDir;
        }

        public void SetSession(string session)
        {
            _sessionInfo.Session = session;
        }

        public void SaveSession()
        {
            Application.Current.Properties[SESSION_INFO_KEY] = JsonConvert.SerializeObject(_sessionInfo);
            AsyncHelper.RunSync(() => Application.Current.SavePropertiesAsync());
        }
    }
}
