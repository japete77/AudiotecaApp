using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using fonoteca.Helpers;
using fonoteca.Models.Api;
using fonoteca.Models.Notifications;
using Newtonsoft.Json;
using RestSharp;

namespace fonoteca.Services
{
    public class NotificationsStore
    {
        private static NotificationsStore _instance;
        private const string DEVICE_TOKEN_KEY = "DeviceTokenKey";
        private const string NOTIFICATIONS_SUBSCRIPTIONS_KEY = "NotificationsSubscriptions";

        private const string NOTIFICATIONS_READ_KEY = "NotificationsRead";
        private List<int> _readNotifications = new List<int>();

        // private List<Topic> _topics;

        private System.Timers.Timer _subscriptionsTimer;
        private System.Timers.Timer _timerUnreadNotifications;

        public static NotificationsStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationsStore();
                }

                return _instance;
            }
        }

        public NotificationsStore()
        {
            // Refresh subscriptions every hour
            _subscriptionsTimer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = 3600000
            };
            _subscriptionsTimer.Elapsed += _subscriptionsTimer_SubscriptionsRefresh;
            _subscriptionsTimer.Start();
        }

        public async Task RefreshNotifications()
        {
            // Read notifications ids
            var notificationsIds = await AudioLibrary.Instance.GetNotificationsIds();
            
            string notificationsKey = null;
            if (Preferences.ContainsKey(NOTIFICATIONS_READ_KEY))
            {
                notificationsKey = Preferences.Get(NOTIFICATIONS_READ_KEY, null);
            }            
            if (!string.IsNullOrEmpty(notificationsKey))
            {
                _readNotifications = JsonConvert.DeserializeObject<List<int>>(notificationsKey);

                // Remove expired notifications (consider only notifications in the list)
                _readNotifications = _readNotifications.Where(w => notificationsIds.Contains(w)).ToList();
            }
            else
            {
                // Is empty so we assume all notifications are read
                _readNotifications = notificationsIds;
                Preferences.Set(NOTIFICATIONS_READ_KEY, JsonConvert.SerializeObject(_readNotifications));                
            }

            if (_timerUnreadNotifications == null)
            {
                // Check unread notifications
                _timerUnreadNotifications = new System.Timers.Timer()
                {
                    AutoReset = false,
                };
                _timerUnreadNotifications.Elapsed += _timerUnreadNotifications_Elapsed;
                _timerUnreadNotifications.Start();
            }
        }

        private void _subscriptionsTimer_SubscriptionsRefresh(object sender, System.Timers.ElapsedEventArgs e)
        {
            AsyncHelper.RunSync(() => RegisterUserNotifications());
        }

        private void _timerUnreadNotifications_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var unreadNotificationsCount = AsyncHelper.RunSync(() => GetUnreadNotificationsCount());

            if (unreadNotificationsCount > 0)
            {
                var msg = unreadNotificationsCount > 1 ?
                    $"Tienes {unreadNotificationsCount} notificaciones sin leer" :
                    $"Tienes {unreadNotificationsCount} notificación sin leer";

                // ShowAlert("Aviso", msg);
            }
        }

        public async Task<List<NotificationModel>> GetNotifications()
        {
            var result = await AudioLibrary.Instance.GetNotifications();

            // Mark notifications as Unread if it doesn´t exists in the unread notifications
            result.ForEach(item =>
            {

                if (_readNotifications.Contains(item.Id))
                {
                    item.TextStyle = FontAttributes.None;
                    item.Header = "";
                }
                else
                {
                    item.TextStyle = FontAttributes.Bold;
                    item.Header = $"no leído";
                }
            });

            return result;
        }

        public async Task<int> GetUnreadNotificationsCount()
        {
            var notificationsId = await AudioLibrary.Instance.GetNotificationsIds();

            var unreadNotificationsCount = notificationsId.Where(w => !_readNotifications.Contains(w)).Count();

            return unreadNotificationsCount;
        }

        public void SetNotificationRead(int id)
        {
            if (!_readNotifications.Contains(id))
            {
                _readNotifications.Add(id);
                Preferences.Set(NOTIFICATIONS_READ_KEY, JsonConvert.SerializeObject(_readNotifications));                
            }
        }

        public string GetDeviceToken()
        {
            if (Preferences.ContainsKey(DEVICE_TOKEN_KEY))
            {
                return Preferences.Get(DEVICE_TOKEN_KEY, "");
            }

            return "";
        }

        public void SaveDeviceToken(string deviceToken)
        {
            // Save device token
            Preferences.Set(DEVICE_TOKEN_KEY, deviceToken);            
        }

        public SNSSubscriptions GetNotificationsSubscriptions()
        {
            if (Preferences.ContainsKey(NOTIFICATIONS_SUBSCRIPTIONS_KEY))
            {
                return JsonConvert.DeserializeObject<SNSSubscriptions>(Preferences.Get(NOTIFICATIONS_SUBSCRIPTIONS_KEY, ""));
            }

            return new SNSSubscriptions
            {
                Subscriptions = new Dictionary<string, string>()
            };
        }

        public void SaveNotificationsSubscriptions(SNSSubscriptions subscriptions)
        {
            // Save device token
            Preferences.Set(NOTIFICATIONS_SUBSCRIPTIONS_KEY, JsonConvert.SerializeObject(subscriptions));            
        }

        public async Task RegisterUserNotifications()
        {
            string deviceToken = GetDeviceToken();

            var notificationsSubscriptions = GetNotificationsSubscriptions();

            var request = new RestRequest("notifications/synch")
            {
                RequestFormat = DataFormat.Json,
                Method = Method.Put
            };

            request.AddJsonBody(new SynchNotificationsRequest
            {
                DeviceToken = deviceToken,
                Platform = DeviceInfo.Platform == DevicePlatform.iOS ? "iOS" : "Android",
                Session = Session.Instance.GetSession(),
                Subscriptions = notificationsSubscriptions
            });

            var response = await ApiClient.Instance.Client.ExecutePutAsync<SynchNotificactionsResponse>(request);

            if (response.IsSuccessful)
            {
                // Save notifications subscriptions
                SaveNotificationsSubscriptions(response.Data.Subscriptions);
            }
            else
            {
                // Error synch subscriptions
            }
        }
    }

    public class SNSSubscriptions
    {
        public string DeviceToken { get; set; }
        public string ApplicationEndPoint { get; set; }
        public Dictionary<string, string> Subscriptions { get; set; }
    }
}
