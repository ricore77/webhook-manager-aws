using WebhookManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebhookManager.Dto;

namespace WebhookManager.Services
{
    public interface IApplicationService : IServiceBase
    {
        Task<string> CreateAsync(ApplicationCreateRequest app);
        void DeleteAllTopics();
        Task<string> NotifyAsync(string appId, string eventName, string message);

        Task<Application> GetApplicationByIdAsync(string id);
    }
}
