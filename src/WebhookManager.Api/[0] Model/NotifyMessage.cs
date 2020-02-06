using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Model
{
    public class NotifyMessage
    {
        public string ApplicationId { get; set; }
        public string NotifyRequestId { get; set; }

        public string Payload { get; set; }
        public string EventName { get; set; }


    }
}
