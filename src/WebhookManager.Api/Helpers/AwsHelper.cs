using Amazon;
using Amazon.APIGateway;
using Amazon.DynamoDBv2;
using Amazon.Lambda;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Helpers
{
    public static class AwsFactory
    {
        public static Config.MyAWSConfig AwsConfig { get; set; }
      
    
        public static T GetClientConfig<T>()
        {

            T checkType = default(T);


            if (typeof(T) == typeof(AmazonSimpleNotificationServiceConfig)) 
            {
                return (T)Convert.ChangeType(
                    new AmazonSimpleNotificationServiceConfig()
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region)
                    }, typeof(T));

            }
            else if (typeof(T) == typeof(AmazonSQSConfig)) 
            {
                return (T)Convert.ChangeType(
                    new AmazonSQSConfig()
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region)
                    }, typeof(T));
            }
            else if (typeof(T) == typeof(AmazonAPIGatewayConfig)) 
            {
                return (T)Convert.ChangeType(
                    new Amazon.APIGateway.AmazonAPIGatewayConfig()
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region)
                    }, typeof(T));
            }

            else if (typeof(T) == typeof(AmazonDynamoDBConfig)) 
            {
                return (T)Convert.ChangeType(
                    new AmazonDynamoDBConfig()
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region)
                }, typeof(T));
            }
            else if (typeof(T) == typeof(AmazonLambdaConfig))
            {
                return (T)Convert.ChangeType(
                    new AmazonLambdaConfig()
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(AwsConfig.Region)
                    }, typeof(T));
            }


            throw new NotImplementedException();
        }

        public static T CreateClient<T>() where T : AmazonServiceClient
        {
           
            if (typeof(T) ==  typeof(AmazonSimpleNotificationServiceClient))
            {
                return (T)Convert.ChangeType(
                    new AmazonSimpleNotificationServiceClient(AwsConfig.AWSCredentials, GetClientConfig<AmazonSimpleNotificationServiceConfig>())
                   , typeof(T));
            }
            else if (typeof(T) == typeof(AmazonSQSClient))
            {
                return (T)Convert.ChangeType(
                    new AmazonSQSClient(AwsConfig.AWSCredentials, GetClientConfig<AmazonSQSConfig>())
                   , typeof(T));
            }
            else if (typeof(T) == typeof(AmazonDynamoDBClient))
            {
                return (T)Convert.ChangeType(
                    new AmazonDynamoDBClient(AwsConfig.AWSCredentials, GetClientConfig<AmazonDynamoDBConfig>())
                   , typeof(T));
            }
            else if (typeof(T) == typeof(AmazonLambdaClient))
            {
                return (T)Convert.ChangeType(
                    new AmazonLambdaClient(AwsConfig.AWSCredentials, GetClientConfig<AmazonLambdaConfig>())
                   , typeof(T));
            }
       
            throw new NotImplementedException();
        }
    }
}
