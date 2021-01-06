using System;
namespace audioteca.Models.Api
{
    public class SubscriptionTitle
    {
        public int Id { get; set; }
        public DateTime PublishingDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
