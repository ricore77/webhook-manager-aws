using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Model;
using WebhookManager.Helpers;
using WebhookManager.Repository;

namespace WebhookManager.Repository
{
    public class ApplicationRepository : DynamoDbRepositoryBase<Application>, IApplicationRepository
    {
        
    }
}
