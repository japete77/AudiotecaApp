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
            return _notifications;
        }

        public async Task RegisterUserNotifications(string deviceToken)
        {
            var credentials = new BasicAWSCredentials(
                AppSettings.Instance.AwsKey,
                AppSettings.Instance.AwsSecret
            );

            var client = new AmazonSimpleNotificationServiceClient(
                credentials,
                Amazon.RegionEndpoint.EUWest1
            );

            // register with SNS to create an endpoint ARN
            var endPointResponse = await client.CreatePlatformEndpointAsync(
                new CreatePlatformEndpointRequest
                {
                    Token = deviceToken,
                    PlatformApplicationArn = Device.RuntimePlatform == Device.iOS ?
                        AppSettings.Instance.AwsPlatformApplicationArnIOS :
                        AppSettings.Instance.AwsPlatformApplicationArnAndroid
                }
            );

            // Retrieve subscriptions
            var subscriptions = await AudioLibrary.Instance.GetUserSubscriptions();

            if (subscriptions == null) subscriptions = new UserSubscriptions { Subscriptions = new List<Models.Api.Subscription>() };

            // Register default subscriptions CAT and General
            subscriptions.Subscriptions.Add(new Models.Api.Subscription { Code = "CAT" });
            subscriptions.Subscriptions.Add(new Models.Api.Subscription { Code = "" });

            // Register subscriptions
            foreach (var code in subscriptions.Subscriptions.Select(s => s.Code))
            {
                var topicArn = AppSettings.Instance.AwsTopicArn;
                topicArn += Device.RuntimePlatform == Device.iOS ? "-ios" : "-android";
                topicArn += string.IsNullOrEmpty(code) ? "" : $"-{code}";

                // Subscribe
                var subscribeResponse = await client.SubscribeAsync(new SubscribeRequest
                {
                    Protocol = "application",
                    Endpoint = endPointResponse.EndpointArn,
                    TopicArn = topicArn
                });
            }
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
}
