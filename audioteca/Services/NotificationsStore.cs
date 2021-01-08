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
        private const int MAX_NOTIFICATIONS = 100;
        private static NotificationsStore _instance;
        private const string DEVICE_TOKEN_KEY = "DeviceTokenKey";
        private const string NOTIFICATIONS_SUBSCRIPTIONS_KEY = "NotificationsSubscriptions";
        private List<Topic> _topics;

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

        private const string NOTIFICATIONS_KEY = "Notifications";
        private List<NotificationModel> _notifications = new List<NotificationModel>();

        public NotificationsStore()
        {
            // Read audiobooks list
            Application.Current.Properties.TryGetValue(NOTIFICATIONS_KEY, out object data);
            if (data != null) _notifications = JsonConvert.DeserializeObject<List<NotificationModel>>(data.ToString());

        }

        public async Task AddNotification(NotificationModel notification)
        {
            _notifications.Insert(0, notification);

            if (_notifications.Count > MAX_NOTIFICATIONS)
            {
                _notifications = _notifications.GetRange(0, MAX_NOTIFICATIONS);
            }

            await SaveNotifications();
        }

        public async Task RemoveNotification(int index)
        {
            _notifications.RemoveAt(index);

            await SaveNotifications();
        }

        public List<NotificationModel> GetNotifications()
        {
            // TODO: Read from DB
            return _notifications;
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
                        await client.DeleteEndpointAsync(new DeleteEndpointRequest { EndpointArn = notificationsSubscriptions.ApplicationEndPoint });
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

                // Save device token and application endpoint created
                notificationsSubscriptions.DeviceToken = deviceToken;
                notificationsSubscriptions.ApplicationEndPoint = endPointResponse.EndpointArn;
            }

            // Retrieve subscriptions
            var subscriptions = await AudioLibrary.Instance.GetUserSubscriptions();

            if (subscriptions == null) subscriptions = new UserSubscriptions { Subscriptions = new List<Models.Api.Subscription>() };
            // Register default subscriptions CAT and General
            subscriptions.Subscriptions.Add(new Models.Api.Subscription { Code = "CAT" });
            subscriptions.Subscriptions.Add(new Models.Api.Subscription { Code = "" });

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
                        topicArn = topicResponse.TopicArn;
                    }

                    // Subscribe
                    var subscribeResponse = await client.SubscribeAsync(new SubscribeRequest
                    {
                        Protocol = "application",
                        Endpoint = notificationsSubscriptions.ApplicationEndPoint,
                        TopicArn = topicArn
                    });

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

        public async Task ShowNotification(int index)
        {
            await Application.Current.MainPage.Navigation.PushAsync(
                new NotificationDetailPage(
                    _notifications.First(),
                    0
                )
            );
        }

        private async Task SaveNotifications()
        {
            Application.Current.Properties[NOTIFICATIONS_KEY] = JsonConvert.SerializeObject(_notifications);
            await Application.Current.SavePropertiesAsync();
        }

    }

    public class SNSSubscriptions
    {
        public string DeviceToken { get; set; }
        public string ApplicationEndPoint { get; set; }
        public Dictionary<string, string> Subscriptions { get; set; }
    }
}
