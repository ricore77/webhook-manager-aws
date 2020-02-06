using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Model
{
    public class Subscription
    {
        
        public List<Event> Events { get; set; }

        public string WebHookUrl { get; set; }

        [DynamoDBHashKey]
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string UserId { get; set; }

        public string Content_Type { get; set; }

        public string Secret { get; set; }
        public string QueueUrl { get; set; }
        public string Name { get; set; }



    }
}
