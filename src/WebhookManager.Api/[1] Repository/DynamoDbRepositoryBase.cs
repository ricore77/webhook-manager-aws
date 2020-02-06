using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Helpers;

namespace WebhookManager.Repository
{
    public class DynamoDbRepositoryBase<T> 
    {

         AmazonDynamoDBClient _dynamoClient;


         DynamoDBContext _dynamodbContext;

        public  virtual AmazonDynamoDBClient DynamoClient
        {
            get
            {
                if (_dynamoClient == null)
                    _dynamoClient = AwsFactory.CreateClient<AmazonDynamoDBClient>();
                return _dynamoClient;
            }
        }


        public virtual DynamoDBContext DynamoDBContext
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

        public virtual async Task CreateAsync(T obj)
        {
            await DynamoDBContext.SaveAsync<T>(obj);

            
        }

        public virtual async Task DeleteAsync(T obj)
        {
            await DynamoDBContext.DeleteAsync<T>(obj);

        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            var app = await DynamoDBContext.LoadAsync<T>(id);
            return app;

        }

        public virtual async Task UpdateAsync(T obj)
        {
            await DynamoDBContext.SaveAsync<T>(obj);

        }
    }
}
