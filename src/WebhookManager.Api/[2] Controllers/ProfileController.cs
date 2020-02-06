//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Amazon.DynamoDBv2;
//using WebhookManager.Model;
//using WebhookManager.Services;
//using Microsoft.Extensions.Logging;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using Amazon.Runtime;
//using WebhookManager.Profile;
//using Amazon;
//using Amazon.ECS;
//using Microsoft.AspNetCore.Mvc.Infrastructure;
//using Microsoft.AspNetCore.Http;

//namespace WebhookManager.Controllers
//{
//    [Route("api/[controller]")]
//    public class ProfileController : Controller
//    {
//        private IAmazonDynamoDB _dynamoDbClient;
//        private IProfileService _service;
//        private readonly ILogger _logger;
//        private static readonly HttpClient client = new HttpClient();
//        private AWSCredentials _awscredential;
//        MyAWSConfig myConfig;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        public ProfileController(MyAWSConfig config, IHttpContextAccessor accessor, IAmazonDynamoDB dynamoDbClient, IProfileService service, ILogger<ProfileController> logger)
//        {
//            try
//            {
//                _logger = logger;
//                _dynamoDbClient = dynamoDbClient;
//                _service = service;
//                myConfig = config;
//                _httpContextAccessor = accessor;

//            }
//            catch (Exception ex)
//            {
//                _logger.LogWarning("$$$$$ ProfileController ^^^^&&&&");
//                _logger.LogWarning("ERROR: " + ex.InnerException);

//                throw;
//            }






//        }
//        // GET api/values
//        [HttpGet("GetAll")]
//        public async Task<List<WebhookManager.Model.Profile>> GetAll()
//        {
//            try
//            {
//                return await _service.GetAll();
//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }

//        }
//        [HttpGet("Test")]
//        public async Task<IActionResult> Test()
//        {
//            try
//            {
//                var x = "hello";
//                string ip, host, msg = "";

//                try
//                {
//                    ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
//                    host = _httpContextAccessor.HttpContext.Request.Host.Value;

//                    _logger.LogInformation($"*** ip {ip} host {host}");

//                }
//                catch (Exception ex)
//                {

//                    _logger.LogCritical("ERROR@: " + ex.InnerException);
//                    throw ex;
//                }



//                client.DefaultRequestHeaders.Accept.Clear();
//                client.DefaultRequestHeaders.Accept.Add(
//                    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
//                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

//                _logger.LogInformation($"*** CHAMANDO A API");
//                _logger.LogInformation($"http://{myConfig.ApiName}/api/Profile/Test");

//                try
//                {
//                    var stringTask = client.GetStringAsync($"http://{myConfig.ApiName}/api/Profile/Test");
//                    msg = await stringTask;


//                }
//                catch (Exception ex)
//                {
//                    _logger.LogCritical("ERROR@: " + ex.InnerException);
//                }
//                try
//                {
//                    _logger.LogInformation($"*** CHAMANDO A API com porta");
//                    _logger.LogInformation($"http://{myConfig.ApiName}:5000/api/Profile/Test");
//                    var stringTask = client.GetStringAsync($"http://{myConfig.ApiName}:5000/api/Profile/Test");
//                    msg = await stringTask;


//                }
//                catch (Exception ex)
//                {
//                    _logger.LogCritical("ERROR@: " + ex.InnerException);
//                }


//                return new ObjectResult($"{msg} seu ip {ip} e seu host {host}");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogInformation("$$$$$ Test ^^^^&&&&");
//                _logger.LogCritical("ERROR@: " + ex.InnerException);
//                throw ex;
//            }

//        }

//        // GET api/values/5
//        [HttpGet("GetProfileById/{id}")]
//        public async Task<WebhookManager.Model.Profile> GetProfileById(string id)
//        {
//            try
//            {

//                return await _service.GetProfileById(id);
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }

//        // POST api/values
//        [HttpPost("CreateProfile")]
//        public void CreateProfile([FromBody]WebhookManager.Model.Profile value)
//        {
//            try
//            {
//                _service.Create(value);
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }

//        // PUT api/values/5
//        [HttpPut("Update")]
//        public void Put([FromBody]WebhookManager.Model.Profile value)
//        {
//            _service.Update(value);

//        }
//        [HttpPut("Like")]
//        public void Like(string myProfile, string profileILike)
//        {
//            _service.Like(myProfile, profileILike);

//        }
//        // DELETE api/values/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }


//    }
//}
