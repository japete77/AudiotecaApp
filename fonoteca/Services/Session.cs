using fonoteca.Exceptions;
using fonoteca.Models.Api;
using fonoteca.Models.Session;
using Newtonsoft.Json;
using RestSharp;

namespace fonoteca.Services
{
    public class Session
    {
        private readonly SessionInfo _sessionInfo = new SessionInfo();
        private string _lastError;
        private const string SESSION_INFO_KEY = "SessionInfo";

        private static Session _instance;
        public static Session Instance
        {
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
            if (Preferences.ContainsKey(SESSION_INFO_KEY))
            {
                _sessionInfo = JsonConvert.DeserializeObject<SessionInfo>(Preferences.Get(SESSION_INFO_KEY, null));
            }
        }

        public async Task<LoginResult> Login(int user, string password)
        {
            return await Task.Run(() =>
            {
                RestRequest request = new RestRequest("login")
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };
                request.AddJsonBody(new LoginRequest { User = user, Password = password });

                var result = ApiClient.Instance.Client.ExecutePost<LoginResult>(request);
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

                return result.Data;
            });
        }

        public async Task ChangePassword(string newPassword)
        {
            await Task.Run(() =>
            {
                RestRequest request = new RestRequest("change-password")
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };
                request.AddJsonBody(new ChangePasswordRequest { Session = _sessionInfo.Session, NewPassword = newPassword });

                var result = ApiClient.Instance.Client.ExecutePost<ChangePasswordRequest>(request);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("No se ha podido cambiar la contraseña");
                }
            });
        }

        public async Task ForgotPassword(string email)
        {
            await Task.Run(() =>
            {
                RestRequest request = new RestRequest("forgot-password")
                {
                    Method = Method.Post,
                    RequestFormat = DataFormat.Json
                };
                request.AddJsonBody(new ForgotPasswordRequest { Email = email });

                var result = ApiClient.Instance.Client.ExecutePost<ForgotPasswordRequest>(request);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("No se ha podido solicitar la recuparción de la contraseña");
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
                if (result.Success)
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
            Preferences.Set(SESSION_INFO_KEY, JsonConvert.SerializeObject(_sessionInfo));
        }
    }
}
