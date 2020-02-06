using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Model;

namespace WebhookManager.Repository
{
    public interface ISubscriptionRepository : IRepositoryBase<Subscription>
    {
        List<string> FindSubscribersByApplication(string ApplicationID);
    }
}
