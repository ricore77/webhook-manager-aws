using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebhookDispatcher.Lambda
{
    public class Helpers
    { 
        public static NotificationMessage NotifyMessageFactory(string subsID, string secret, string msgBody)
        {
            return new NotificationMessage()
            {
                SubscriptionId = subsID,
                NotifyId = Guid.NewGuid().ToString(),
                Signature = Sign(msgBody, secret),
                Payload = (string)JObject.Parse(msgBody)["Payload"],
                Timestamp = DateTime.Now.ToLongTimeString(),
                ApplicationId = (string)JObject.Parse(msgBody)["ApplicationId"],
                EventName = (string)JObject.Parse(msgBody)["EventName"],
                NotifyRequestId = (string)JObject.Parse(msgBody)["NotifyRequestId"]

            };
        }

        public static String Sign(String data, String key)
        {
            KeyedHashAlgorithm algorithm = new HMACSHA1();

            Encoding encoding = new UTF8Encoding();
            algorithm.Key = encoding.GetBytes(key);
            return BitConverter.ToString((algorithm.ComputeHash(
            encoding.GetBytes(data.ToCharArray())))).Replace("-", "");
        }

        public static Func<IServiceProvider> ConfigureServices = () =>
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return serviceCollection.BuildServiceProvider();
        };

    }
}
