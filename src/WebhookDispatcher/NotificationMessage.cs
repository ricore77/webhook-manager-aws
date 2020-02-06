using System;
using System.Collections.Generic;
using System.Text;

namespace WebhookDispatcher.Lambda
{
    public class NotificationMessage
    {
         
        public string NotifyId { get; set; }

        public string Timestamp { get; set; }

        public string Payload { get; set; }

        public string ApplicationId { get; set; }
        public string EventName { get; set; }


        public string SubscriptionId { get; set; }

        public string Signature { get; set; }
        public string NotifyRequestId { get; set; }

        

    }
    public class NotificationRequest
    {
        public string ApplicationId { get; set; }
        public string Payload { get; set; }
        public string TopicArn { get; set; }
        public string EventName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Id { get; set; }


    }
    public class Notification
    {
       

        public string Status { get; set; }

        public string Id { get; set; }

        public string NotificationRequestId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Payload { get; set; }

        public string ApplicationId { get; set; }
        public string EventName { get; set; }

        public string SubscriptionId { get; set; }

        public string Signature { get; set; }
        public string Webhook { get; set; }




    }
}
