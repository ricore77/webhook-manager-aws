using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Config
{
    public class MyAWSConfig
    {
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Region { get; set; }
        public string ApiName { get; set; }
        public AWSCredentials AWSCredentials { get; set; }


        


    }
}
