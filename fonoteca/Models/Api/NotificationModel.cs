﻿namespace fonoteca.Models.Api
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Code { get; set; }
        public string ContentId { get; set; }
        public FontAttributes TextStyle { get; set; }
        public string Header { get; set; }
    }
}
