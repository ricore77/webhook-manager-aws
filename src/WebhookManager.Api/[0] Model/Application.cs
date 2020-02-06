using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace WebhookManager.Model
{
    public class Application
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [DynamoDBHashKey]
        public string Id { get; set; }

        public List<Event> Events { get; set; }

        

    }
}
