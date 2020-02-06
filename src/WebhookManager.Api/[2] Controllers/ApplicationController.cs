using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using WebhookManager.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebhookManager.Profile.Extensions;
using WebhookManager.Helpers;
using WebhookManager.Services;
using System.Net;
using WebhookManager.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebhookManager.Controllers
{
    [Route("api/application")]
    public class ApplicationController : Controller
    {
        private IApplicationService ApplicationService { get; }
        public ApplicationController(IApplicationService service)
        {
            ApplicationService = service;
        }
    

        [HttpPost("cleanUp")]
        public IActionResult CleanUp()
        {
            ApplicationService.DeleteAllTopics();
            return Ok();

        }

        [HttpPost("notify")]
        public async Task<string> Notify(string appId, string eventName, string message)
        {
            var result = await ApplicationService.NotifyAsync(appId, eventName, message);
            return result;
        }


        [HttpPost]
        public async Task<string> Post([FromBody] ApplicationCreateRequest Webhook)
        {

            string appID = await ApplicationService.CreateAsync(Webhook);
            return appID;

        }


        [HttpGet("get/{id}")]

        public async Task<Application> GetApplicationById(string id)
        {
            return await ApplicationService.GetApplicationByIdAsync(id);
        }


    }
}
