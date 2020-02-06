using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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
using Amazon.Lambda;
using System.Net;
using Amazon.DynamoDBv2.Model;
using System.Text.Json;
using WebhookManager.Model;
using WebhookManager.Services;
using WebhookManager.Repository;
using WebhookManager.Dto;
using AutoMapper;

namespace WebhookManager.Services
{
    public class ApplicationService : IApplicationService
    {

        private IApplicationRepository Repository { get; set; }
        private INotificationRequestRepository NotificationRequestRepository { get; set; }
        private IMapper Mapper { get; set; }


        public ApplicationService(IApplicationRepository repository, INotificationRequestRepository notificationRequestRepository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
            NotificationRequestRepository = notificationRequestRepository;
        }


        private void ValidateNotification(string appId, string eventName, string message)
        {
            var exceptionToThrow = new WebhookManagerException("Error when notifying message!");

            if (String.IsNullOrEmpty(appId))
                exceptionToThrow.Errors.Add($"Parameter appId is mandatory!");

            if (String.IsNullOrEmpty(eventName))
                exceptionToThrow.Errors.Add($"Parameter eventName is mandatory!");

            if (String.IsNullOrEmpty(message))
                exceptionToThrow.Errors.Add($"Parameter message is mandatory!");

            if (exceptionToThrow.Errors.Count > 0) throw exceptionToThrow;

        }

        public async Task<string> NotifyAsync(string appId, string eventName, string message)
        {
            ValidateNotification(appId, eventName, message);

            string notifyRequestId = Guid.NewGuid().ToString();
            AmazonSimpleNotificationServiceClient clientSNS = AwsFactory.CreateClient<AmazonSimpleNotificationServiceClient>();

            var app = await Repository.GetByIdAsync(appId);
            Event targetEvent = app.Events.ToList().Where(e => e.EventName == eventName).FirstOrDefault();


            var response = await clientSNS.PublishAsync(new PublishRequest
            {
                Message = JsonSerializer.Serialize<NotifyMessage>(new NotifyMessage
                {
                    EventName = eventName,
                    ApplicationId = appId,
                    Payload = message,
                    NotifyRequestId = notifyRequestId
                }),

                TargetArn = targetEvent.TopicArn
            });

            await NotificationRequestRepository.CreateAsync(new NotificationRequest
            {
                ApplicationId = appId,
                EventName = eventName,
                Id = notifyRequestId,
                Payload = message,
                TopicArn = targetEvent.TopicArn,
                Timestamp = DateTime.Now.ToUniversalTime()
            });



            return response.MessageId;

        }

        public async Task<string> CreateAsync(ApplicationCreateRequest appRequest)
        {

            Application app = Mapper.Map<Application>(appRequest);

            app.Id = Guid.NewGuid().ToString();

            await CreateTopics(app);

            await Repository.CreateAsync(app);

            return app.Id;
        }

        public void DeleteAllTopics()
        {
            AmazonSimpleNotificationServiceClient clientSNS = AwsFactory.CreateClient<AmazonSimpleNotificationServiceClient>();
            AmazonSQSClient clientSQS = AwsFactory.CreateClient<AmazonSQSClient>();
            AmazonLambdaClient lambdaClient = AwsFactory.CreateClient<AmazonLambdaClient>();
           

            var topics = clientSNS.ListTopicsAsync();
            // var subs = clientSNS.ListSubscriptionsAsync(new ListSubscriptionsRequest());
            var filas = clientSQS.ListQueuesAsync("subs");

            filas.Result.QueueUrls.ForEach(i =>
            {
                var deleted = clientSQS.DeleteQueueAsync(i);
                if (deleted.Result.HttpStatusCode != HttpStatusCode.OK)
                {
                    int x = 0;
                }
            });

            string nextToken = "";
            do
            {
                var subs = clientSNS.ListSubscriptionsAsync(new ListSubscriptionsRequest(nextToken));

                subs.Result.Subscriptions.ForEach(i =>
                {
                    var deleted = clientSNS.UnsubscribeAsync(i.SubscriptionArn);
                });

                nextToken = subs.Result.NextToken;

            } while (!String.IsNullOrEmpty(nextToken));



            var mapper = lambdaClient.ListEventSourceMappingsAsync(new Amazon.Lambda.Model.ListEventSourceMappingsRequest
            {
                FunctionName = "WebhookDispatcher"
            });

            mapper.Result.EventSourceMappings.ToList().ForEach(i =>
            {
                var result = lambdaClient.DeleteEventSourceMappingAsync(new Amazon.Lambda.Model.DeleteEventSourceMappingRequest()
                {
                    UUID = i.UUID
                });
                if (result.Result.HttpStatusCode != HttpStatusCode.OK)
                {
                    int x = 0;
                }
            });


            topics.Result.Topics.ForEach(i =>
           {
               var deleted = clientSNS.DeleteTopicAsync(new DeleteTopicRequest()
               {
                   TopicArn = i.TopicArn
               });

           });

        }

        private async Task<Application> CreateTopics(Application app)
        {
            string queueName = $"queue-{app.Name.Replace(" ", "")}-{app.Id}-subs";

            AmazonSimpleNotificationServiceClient clientSNS = AwsFactory.CreateClient<AmazonSimpleNotificationServiceClient>();



            //create topics and subscribe them
            await app.Events.ToList().ForEachAsync(e =>
            {
                string topic_name = $"topic-{app.Name.Replace(" ", "")}-event-{e.EventName}";

                CreateTopicResponse topicResponse = clientSNS.CreateTopicAsync(new CreateTopicRequest(topic_name)).Result;

                if (topicResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"Error creating topic {topic_name}");


                e.TopicArn = topicResponse.TopicArn;

            });

            return app;
        }

        public async Task<Application> GetApplicationByIdAsync(string id)
        {
            return await Repository.GetByIdAsync(id); 
        }


    }
}
