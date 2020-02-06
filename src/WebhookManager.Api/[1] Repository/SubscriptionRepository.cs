using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Model;

namespace WebhookManager.Repository
{
    public class SubscriptionRepository : DynamoDbRepositoryBase<Subscription>, ISubscriptionRepository
    {
        public new async Task CreateAsync(Subscription subscription)
        {
            var EventAggregateBatch = DynamoDBContext.CreateBatchWrite<EventAggregate>();
            var SubscriptionBatch = DynamoDBContext.CreateBatchWrite<Model.Subscription>();


            subscription.Events.ForEach(e =>
            {
                EventAggregateBatch.AddPutItem(
                          new EventAggregate
                          {

                              SubscriptionId = subscription.Id,
                              ApplicationId = subscription.ApplicationId,
                              EventName = e.EventName
                          }
                          );
            });

            SubscriptionBatch.AddPutItem(subscription);
            MultiTableBatchWrite batch = DynamoDBContext.CreateMultiTableBatchWrite(EventAggregateBatch, SubscriptionBatch);

            await batch.ExecuteAsync();


        }
        public List<string> FindSubscribersByApplication(string appId)
        {

            try
            {
                List<string> subscriptions = new List<string>();
                var query = base.DynamoClient.QueryAsync(new QueryRequest
                {
                    TableName = "EventAggregate",
                    IndexName = "ApplicationId-Index",
                    Select = Select.SPECIFIC_ATTRIBUTES,
                    ProjectionExpression = "SubscriptionId, EventName, ApplicationId",
                    KeyConditionExpression = "ApplicationId  = :ApplicationId",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                        {
                            {":ApplicationId", new AttributeValue {S = appId}}
                        }
                });

                if (query != null)
                {
                    if (query.Result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {

                        query.Result.Items.ForEach(itens =>
                        {
                            subscriptions.Add(itens.Where(k => k.Key == "SubscriptionId").FirstOrDefault().Value?.S);

                        });
                    }

                }
                return subscriptions.Distinct().ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
