using System;
using System.Collections.Generic;

namespace audioteca.Models.Api
{
    public class SubscriptionTitleResult
    {
        public List<SubscriptionTitle> Titles { get; set; }
        public int Total { get; set; }
    }
}
