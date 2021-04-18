using Acr.UserDialogs;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using audioteca.Helpers;
using audioteca.Models.Api;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace audioteca.Services
{
    public class NotificationsStore
    {
        private static NotificationsStore _instance;
        private const string DEVICE_TOKEN_KEY = "DeviceTokenKey";
        private const string NOTIFICATIONS_SUBSCRIPTIONS_KEY = "NotificationsSubscriptions";

        private const string NOTIFICATIONS_READ_KEY = "NotificationsRead";
        private List<int> _readNotifications = new List<int>();

        private List<Topic> _topics;

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
            Application.Current.Properties.TryGetValue(NOTIFICATIONS_READ_KEY, out object data);
            if (data != null)
            {
                _readNotifications = JsonConvert.DeserializeObject<List<int>>(data.ToString());

                // Remove expired notifications (consider only notifications in the list)
                _readNotifications = _readNotifications.Where(w => notificationsIds.Contains(w)).ToList();
            }
            else
            {
                // Is empty so we assume all notifications are read
                _readNotifications = notificationsIds;
                Application.Current.Properties[NOTIFICATIONS_READ_KEY] = JsonConvert.SerializeObject(_readNotifications);
            }
            await Application.Current.SavePropertiesAsync();

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
                UserDialogs.Instance.Alert(
                    new AlertConfig
                    {
                        Title = "Aviso",
                        Message = msg,
                        OkText = "Aceptar"
                    }
                );
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

        public async Task SetNotificationRead(int id)
        {
            if (!_readNotifications.Contains(id))
            {
                _readNotifications.Add(id);
                Application.Current.Properties[NOTIFICATIONS_READ_KEY] = JsonConvert.SerializeObject(_readNotifications);
                await Application.Current.SavePropertiesAsync();
            }
        }

        public string GetDeviceToken()
        {
            if (Application.Current.Properties.ContainsKey(DEVICE_TOKEN_KEY))
            {
                return (string)Application.Current.Properties[DEVICE_TOKEN_KEY];
            }

            return "";
        }

        public async Task SaveDeviceToken(string deviceToken)
        {
            // Save device token
            Application.Current.Properties[DEVICE_TOKEN_KEY] = deviceToken;

            await Application.Current.SavePropertiesAsync();
        }

        public SNSSubscriptions GetNotificationsSubscriptions()
        {
            if (Application.Current.Properties.ContainsKey(NOTIFICATIONS_SUBSCRIPTIONS_KEY))
            {
                return JsonConvert.DeserializeObject<SNSSubscriptions>(Application.Current.Properties[NOTIFICATIONS_SUBSCRIPTIONS_KEY].ToString());
            }

            return new SNSSubscriptions
            {
                Subscriptions = new Dictionary<string, string>()
            };
        }

        public async Task SaveNotificationsSubscriptions(SNSSubscriptions subscriptions)
        {
            // Save device token
            Application.Current.Properties[NOTIFICATIONS_SUBSCRIPTIONS_KEY] = JsonConvert.SerializeObject(subscriptions);

            await Application.Current.SavePropertiesAsync();
        }

        public async Task<bool> TopicExists(string topic, AmazonSimpleNotificationServiceClient client)
        {
            string nextToken = null;
            if (_topics == null)
            {
                _topics = new List<Topic>();
                do
                {
                    var topicsResponse = await client.ListTopicsAsync(nextToken);
                    _topics.AddRange(topicsResponse.Topics);
                    nextToken = topicsResponse.NextToken;
                } while (nextToken != null);
            }

            return _topics.Any(a => a.TopicArn == topic);
        }

        public async Task RegisterUserNotifications()
        {
            string deviceToken = GetDeviceToken();

            var notificationsSubscriptions = GetNotificationsSubscriptions();

            if (string.IsNullOrEmpty(deviceToken)) return;

            var credentials = new BasicAWSCredentials(
                AppSettings.Instance.AwsKey,
                AppSettings.Instance.AwsSecret
            );

            var client = new AmazonSimpleNotificationServiceClient(
                credentials,
                Amazon.RegionEndpoint.EUWest1
            );

            if (string.IsNullOrEmpty(notificationsSubscriptions.ApplicationEndPoint) ||
                notificationsSubscriptions.DeviceToken != deviceToken)
            {

                // **********************************************
                // de-register old endpoint and all subscriptions
                if (!string.IsNullOrEmpty(notificationsSubscriptions.ApplicationEndPoint))
                {
                    try
                    {
                        var response = await client.DeleteEndpointAsync(new DeleteEndpointRequest { EndpointArn = notificationsSubscriptions.ApplicationEndPoint });

                        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                        {
                            ShowAlert("Debug", $"Error eliminando endpoint: {response.HttpStatusCode}");
                        }
                    }
                    catch { /*Silent error in case endpoint doesn´t exist */ }

                    notificationsSubscriptions.ApplicationEndPoint = null;

                    foreach (var sub in notificationsSubscriptions.Subscriptions)
                    {
                        try
                        {
                            await client.UnsubscribeAsync(sub.Value);
                        }
                        catch { /*Silent error in case endpoint doesn´t exist */ }
                    }

                    notificationsSubscriptions.Subscriptions.Clear();
                }

                // register with SNS to create a new endpoint
                var endPointResponse = await client.CreatePlatformEndpointAsync(
                    new CreatePlatformEndpointRequest
                    {
                        Token = deviceToken,
                        PlatformApplicationArn = Device.RuntimePlatform == Device.iOS ?
                            AppSettings.Instance.AwsPlatformApplicationArnIOS :
                            AppSettings.Instance.AwsPlatformApplicationArnAndroid
                    }
                );

                if (endPointResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    ShowAlert("Debug", $"Error registrando endpoint: {endPointResponse.HttpStatusCode}, {endPointResponse.ResponseMetadata}");
                }

                // Save device token and application endpoint created
                notificationsSubscriptions.DeviceToken = deviceToken;
                notificationsSubscriptions.ApplicationEndPoint = endPointResponse.EndpointArn;
            }

            // Retrieve subscriptions
            var subscriptions = await AudioLibrary.Instance.GetUserSubscriptions(false);

            if (subscriptions == null) subscriptions = new UserSubscriptions { Subscriptions = new List<Models.Api.Subscription>() };

            // Register non existings subscriptions
            var subscriptionsCodes = subscriptions.Subscriptions.Select(s => s.Code).ToList();
            foreach (var code in subscriptionsCodes)
            {
                if (!notificationsSubscriptions.Subscriptions.ContainsKey(code))
                {

                    var topicArn = AppSettings.Instance.AwsTopicArn;
                    topicArn += string.IsNullOrEmpty(code) ? "" : $"-{code}";

                    if (!await TopicExists(topicArn, client))
                    {
                        var topicResponse = await client.CreateTopicAsync(new CreateTopicRequest { Name = $"{AppSettings.Instance.AwsTopicName}-{code}" });

                        if (topicResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                        {
                            ShowAlert("Debug", $"Error creando topic: {topicResponse.HttpStatusCode}, {topicResponse.ResponseMetadata}");
                        }

                        topicArn = topicResponse.TopicArn;
                    }


                    // Subscribe
                    var subscribeResponse = await client.SubscribeAsync(new SubscribeRequest
                    {
                        Protocol = "application",
                        Endpoint = notificationsSubscriptions.ApplicationEndPoint,
                        TopicArn = topicArn
                    });

                    if (subscribeResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    {
                        ShowAlert("Debug", $"Error creando suscripción: {subscribeResponse.HttpStatusCode}, {subscribeResponse.ResponseMetadata}");
                    }

                    // Add to the list
                    notificationsSubscriptions.Subscriptions.Add(code, subscribeResponse.SubscriptionArn);
                }
            }

            // Remove subscriptions not in user list
            var currentSubscriptions = notificationsSubscriptions.Subscriptions.ToList();
            foreach (var subs in currentSubscriptions)
            {
                if (!subscriptionsCodes.Contains(subs.Key))
                {
                    try
                    {
                        await client.UnsubscribeAsync(subs.Value);
                    }
                    catch { /*Silent error in case endpoint doesn´t exist */ }

                    notificationsSubscriptions.Subscriptions.Remove(subs.Key);
                }
            }

            // Save notifications subscriptions
            await SaveNotificationsSubscriptions(notificationsSubscriptions);
        }

        private void ShowAlert(string title, string text)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                await App.Current.MainPage.DisplayAlert(title, text, "Cerrar");
            });
        }
    }

    public class SNSSubscriptions
    {
        public string DeviceToken { get; set; }
        public string ApplicationEndPoint { get; set; }
        public Dictionary<string, string> Subscriptions { get; set; }
    }
}
