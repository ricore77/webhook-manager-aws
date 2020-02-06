using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using WebhookManager.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebhookManager.Profile.Extensions;
using WebhookManager.Helpers;
using Amazon.APIGateway;
using WebhookManager.Services;
using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace WebhookManager.Controllers
{
    [ApiController]
    [Route("api/subscription")]
    public class SubscriptionController : Controller
    {
        #region Properties


        public IHttpClientFactory ClientFactory { get; set; }
        public ISubscriptionService SubscriptionService { get; set; }

        #endregion Properties


        public SubscriptionController(IHttpClientFactory clientFactory, ISubscriptionService subscriptionService)
        {
            ClientFactory = clientFactory;
            SubscriptionService = subscriptionService;

        }

        // GET: api/values
        [HttpGet()]
        [Route("application/{id}/subscribers")]
        public List<string> GetAllSubscribers([FromRoute]string id)
        {
            return SubscriptionService.FindSubscribersByApplication(id);
        }



        [HttpPost()]
        public async Task<string> Post(Model.Subscription subscription)
        {
            return await SubscriptionService.Create(subscription);
        }


        [HttpPost("confirmation")]
        public IActionResult ConfirmationMock(string value)
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = JsonConvert.DeserializeObject<NotificationMessage>(reader.ReadToEnd());

                bool valid = ValidateSignature(body.Payload, "12345", body.Signature);
            }

            return new OkObjectResult("OK");

        }

        private bool ValidateSignature(String payload, String secret, string signedPayload)
        {
            KeyedHashAlgorithm algorithm = new HMACSHA1();

            Encoding encoding = new UTF8Encoding();
            algorithm.Key = encoding.GetBytes(secret);


            string digest = BitConverter.ToString(algorithm.ComputeHash(encoding.GetBytes(payload.ToCharArray()))).Replace("-", "");

            return digest.Equals(signedPayload);


        }

       
    }

    //TODO: REMOVER
    public class NotificationMessage
    {

        public string NotifyId { get; set; }

        public string Timestamp { get; set; }

        public string Payload { get; set; }

        public string ApplicationId { get; set; }
        public string EventName { get; set; }


        public string SubscriptionId { get; set; }

        public string Signature { get; set; }


    }
}
