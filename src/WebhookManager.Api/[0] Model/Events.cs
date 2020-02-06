using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Model
{
    public class Event
    {
        
        public string EventName { get; set; }

        public string TopicArn { get; set; }


    }
}
