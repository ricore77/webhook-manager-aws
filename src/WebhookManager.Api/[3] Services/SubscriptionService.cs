using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using WebhookManager.Profile.Extensions;
using WebhookManager.Helpers;
using WebhookManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Repository;
using WebhookManager.Dto;

namespace WebhookManager.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        #region Properties

        private ISubscriptionRepository Repository { get; set; }
        private IApplicationRepository ApplicationRepository { get; set; }



        public SubscriptionService(ISubscriptionRepository repository, IApplicationRepository appRepository)
        {
            Repository = repository;
            ApplicationRepository = appRepository;
        }
        #endregion Properties
        public async Task<string> Create(Model.Subscription subscription)
        {

            subscription.Id = Guid.NewGuid().ToString();
            string queueName = $"subs-{subscription.Id}";

            //create queue
            var queueResponse = await CreateQueueAsync(queueName, subscription.Id);
            subscription.QueueUrl = queueResponse.QueueUrl;

            //set lambda subs
            await ConfigureLambdaWithQueueAsync(queueName);

            //Get topicsARN
            var app = await ApplicationRepository.GetByIdAsync(subscription.ApplicationId);


            if (app != null)
            {
                //getting ARN
                var events = from subEvent in subscription.Events
                             join appEvent in app.Events
                             on subEvent.EventName equals appEvent.EventName
                             select new Event
                             {

                                 EventName = appEvent.EventName,
                                 TopicArn = appEvent.TopicArn

                             };



                await events.ToList().ForEachAsync(e =>
                {

                    subscription.Events.RemoveAll(y => y.EventName == e.EventName);
                    subscription.Events.Add(e);


                    var sub = AwsFactory.CreateClient<AmazonSimpleNotificationServiceClient>()
                                            .SubscribeQueueToTopicsAsync(
                                                    new List<string>() { e.TopicArn },
                                                    AwsFactory.CreateClient<AmazonSQSClient>(),
                                                    queueResponse.QueueUrl);

                    sub.Result.ToList().ForEach(s =>

                    {
                        try
                        {
                            var setting = AwsFactory.CreateClient<AmazonSimpleNotificationServiceClient>().SetSubscriptionAttributesAsync(new SetSubscriptionAttributesRequest
                            {
                                AttributeName = "RawMessageDelivery",
                                AttributeValue = "true",
                                SubscriptionArn = sub.Result.FirstOrDefault().Value
                            });
                        }
                        catch (Exception)
                        {
                            throw new WebhookManagerException("Error setting Subscription Attribute");
                        }
                    }
                    );


                });

                await Repository.CreateAsync(subscription);
            }
            return subscription.Id;



        }

        public async void FindById(string subsID)
        {
            Model.Subscription subscription = await Repository.GetByIdAsync(subsID);

        }

        public List<string> FindSubscribersByApplication(string appId)
        {


            return Repository.FindSubscribersByApplication(appId);

        }

        public void SendConfirmation(string subscribeId)
        {

        }
        public void RequestConfirmation(string subscribeId)
        {

        }
        public void ConfirmeSubscriber(string subscribeId)
        {

        }
        public void UnSubscribe(List<EventDto> events)
        {

        }
        public void Update(string subscribeId)
        {

        }

        async Task ConfigureLambdaWithQueueAsync(string queueName)
        {
            string queueArn = null;

            AmazonSQSClient sqsClient = AwsFactory.CreateClient<AmazonSQSClient>();

            GetQueueUrlRequest queueUrlReq = new GetQueueUrlRequest();
            queueUrlReq.QueueName = queueName;

            GetQueueUrlResponse getQueueUrlResp = await sqsClient.GetQueueUrlAsync(queueUrlReq);
            GetQueueAttributesRequest queueAttribReq = new GetQueueAttributesRequest();

            queueAttribReq.AttributeNames.Add(QueueAttributeName.QueueArn);
            queueAttribReq.QueueUrl = getQueueUrlResp.QueueUrl;

            var queueAttribResp = await sqsClient.GetQueueAttributesAsync(queueAttribReq);
            queueArn = queueAttribResp.QueueARN;

            AmazonLambdaClient lambdaClient = AwsFactory.CreateClient<AmazonLambdaClient>();

            CreateEventSourceMappingRequest eventMappingReq = new CreateEventSourceMappingRequest();
            eventMappingReq.FunctionName = "WebhookDispatcher";
            eventMappingReq.BatchSize = 10;
            eventMappingReq.Enabled = true;
            eventMappingReq.EventSourceArn = queueArn;

            await lambdaClient.CreateEventSourceMappingAsync(eventMappingReq);

        }
        async Task<CreateQueueResponse> CreateQueueAsync(string queueName, string SubscriptionId)
        {


            CreateQueueRequest deadLetterRequest = new CreateQueueRequest(string.Concat(queueName, "-deadletter"));
            deadLetterRequest.Attributes = new Dictionary<string, string>();
            deadLetterRequest.Attributes.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, "20");
            deadLetterRequest.Attributes.Add(QueueAttributeName.MessageRetentionPeriod, "864000");

            string deadLetterArn = null;

            AmazonSQSClient sqsClient = AwsFactory.CreateClient<AmazonSQSClient>();

            var createResponse = await sqsClient.CreateQueueAsync(deadLetterRequest);
            GetQueueAttributesRequest queueReq = new GetQueueAttributesRequest();
            queueReq.QueueUrl = createResponse.QueueUrl;
            queueReq.AttributeNames.Add(QueueAttributeName.All);
            var queueAttribs = await sqsClient.GetQueueAttributesAsync(queueReq);
            deadLetterArn = queueAttribs.QueueARN;



            string redrivePolicy = $"{{\"deadLetterTargetArn\":\"{deadLetterArn}\",\"maxReceiveCount\":5}}";

            CreateQueueRequest createQueueRequest = new CreateQueueRequest();

            createQueueRequest.QueueName = queueName;
            createQueueRequest.Attributes = new Dictionary<string, string>();
            createQueueRequest.Attributes.Add(QueueAttributeName.RedrivePolicy, redrivePolicy);
            createQueueRequest.Attributes.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, "20");

            //createQueueRequest.Attributes.Add("trigger-id", SubscriptionId);
            CreateQueueResponse queueResponse = await sqsClient.CreateQueueAsync(createQueueRequest);

            return queueResponse;

        }
    }
}
