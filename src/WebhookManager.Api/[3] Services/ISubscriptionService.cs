using WebhookManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookManager.Services
{
    public interface ISubscriptionService : IServiceBase
    {
        Task<string> Create(Subscription subscription);
        void FindById(string subsID);
        List<string> FindSubscribersByApplication(string appId);

    }
}
