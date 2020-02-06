using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace WebhookDispatcher.Lambda
{
    public class Function
    {
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        #region Properties
        private DynamoDBContext _dynamodbContext;
        private Subscription Subscription { get; set; }

        private string lastKey;
        static IServiceProvider services;
        public IHttpClientFactory ClientFactory { get; set; }
        private AmazonDynamoDBClient _dynamoClient;


        private AmazonDynamoDBClient DynamoClient
        {
            get
            {
                if (_dynamoClient == null)
                    _dynamoClient = new AmazonDynamoDBClient();
                return _dynamoClient;
            }
        }

        private DynamoDBContext DynamoDBContext
        {
            get
            {
                if (_dynamodbContext == null)
                {
                    _dynamodbContext = new DynamoDBContext(DynamoClient);


                }
                return _dynamodbContext;
            }

        }

        #endregion Properties



        static Function()
        {
            services = Helpers.ConfigureServices();
        }

        //TODO: Criar Notification status de erro
        public void FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            try
            {

                ClientFactory = services.GetService<IHttpClientFactory>();

                context.Logger.LogLine($"@@@ Beginning to process {sqsEvent.Records.Count} records...");

                sqsEvent.Records.ForEach(async m =>
                {
                    string subsID = m.EventSourceArn.Split("subs-")[1];

                    //Cache
                    if (lastKey != subsID)
                    {
                        Subscription = GetSubscriptionById(subsID);
                        lastKey = subsID;
                    }


                    if (Subscription != null)
                    {

                        var message = Helpers.NotifyMessageFactory(Subscription.Id, Subscription.Secret, m.Body);
                        await CallWebHook(context, message);

                        context.Logger.LogLine($"End CreateNotification");

                    }

                }
                );
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"ERRO:{ex.Message}");
                throw ex;
            }


        }

        private async Task CallWebHook(ILambdaContext context, NotificationMessage message)
        {
            context.Logger.LogLine($"@@@ Calling Webhook {Subscription.WebHookUrl}");

            var request = new HttpRequestMessage(HttpMethod.Post, Subscription.WebHookUrl);

            var stringContent = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            request.Content = stringContent;

            HttpClient clientRest = ClientFactory.CreateClient();
            clientRest.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            HttpResponseMessage response = await clientRest.SendAsync(request);
            context.Logger.LogLine($"@@@ Called Webhook {Subscription.WebHookUrl}");


            string execStatus = "Executed";

            if (!response.IsSuccessStatusCode)
            {
                context.Logger.LogLine($"#### Erro: calling Webhook {Subscription.WebHookUrl}");
                execStatus = "Error";
            }



            await NotificationFactory(context, message, execStatus);

            context.Logger.LogLine($"Begin CreateNotification: message.NotifyId {message.NotifyId}");

        }

        private async Task NotificationFactory(ILambdaContext context, NotificationMessage message, string execStatus)
        {
            await CreateNotification(new Notification
            {
                ApplicationId = message.ApplicationId,
                EventName = message.EventName,
                Id = message.NotifyId,
                Payload = message.Payload,
                Signature = message.Signature,
                Status = execStatus,
                SubscriptionId = Subscription.Id,
                Timestamp = DateTime.Now.ToUniversalTime(),
                NotificationRequestId = message.NotifyRequestId,
                Webhook = Subscription.WebHookUrl
            }, context);
        }

        private async Task CreateNotification(Notification notification, ILambdaContext context)
        {
            try
            {
                context.Logger.LogLine($"*** Begin SaveAsync with RUN ***");

                await DynamoDBContext.SaveAsync<Notification>(notification);

                context.Logger.LogLine($"*** End SaveAsync ***");



            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"*** Erro SaveAsync ***" + ex.Message);

                throw;
            }


        }
        private Subscription GetSubscriptionById(string subsID)
        {
            Subscription subscription = new Subscription();
            var query = DynamoClient.QueryAsync(new QueryRequest
            {
                TableName = "Subscription",
                Select = Select.SPECIFIC_ATTRIBUTES,
                ProjectionExpression = "Id,Content_Type,Secret,WebHookUrl",
                KeyConditionExpression = "Id = :Id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                        {
                            {":Id", new AttributeValue {S = subsID}}
                        }
            });

            if (query != null)
            {
                if (query.Result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {

                    query.Result.Items.ForEach(itens =>
                    {
                        subscription.Id = itens.Where(k => k.Key == "Id").FirstOrDefault().Value?.S;
                        subscription.Content_Type = itens.Where(k => k.Key == "Content_Type").FirstOrDefault().Value?.S;
                        subscription.Secret = itens.Where(k => k.Key == "Secret").FirstOrDefault().Value?.S;
                        subscription.WebHookUrl = itens.Where(k => k.Key == "WebHookUrl").FirstOrDefault().Value?.S;
                    });
                }

            }
            return subscription;
        }



    }




}