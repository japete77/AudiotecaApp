using fonoteca.Services;

namespace fonoteca.Models.Notifications
{
    public class SynchNotificationsRequest
    {
        public string Session { get; set; }
        public string DeviceToken { get; set; }
        public string Platform { get; set; }
        public SNSSubscriptions Subscriptions { get; set; }
    }
}
