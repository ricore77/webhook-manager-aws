using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Model
{
    public class NotificationRequest
    {
        public string ApplicationId { get; set; }
        public string Payload { get; set; }
        public string TopicArn { get; set; }
        public string EventName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Id { get; set; }


    }

    
}
