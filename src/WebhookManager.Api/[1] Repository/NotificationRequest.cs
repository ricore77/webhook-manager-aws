using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Model;

namespace WebhookManager.Repository
{
    public class NotificationRequestRepository : DynamoDbRepositoryBase<NotificationRequest>, INotificationRequestRepository
    {
    }
}
