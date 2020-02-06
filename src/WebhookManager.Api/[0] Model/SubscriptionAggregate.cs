using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Model
{
    [DynamoDBTable("EventAggregate")]
    public class EventAggregate
    {
        public string ApplicationId { get; set; }
        public string SubscriptionId { get; set; }

        public string EventId { get; set; }
        public string EventName { get; set; }


    }
}
