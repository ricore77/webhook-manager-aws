using System;
using System.Collections.Generic;
using System.Text;

namespace WebhookDispatcher.Lambda
{
    public class Subscription
    {
         
        public string WebHookUrl { get; set; }

        public string Id { get; set; }

        public string Content_Type { get; set; }

        public string Secret { get; set; }
        public string QueueUrl { get; set; }

    }
}
