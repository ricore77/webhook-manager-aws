using Amazon.Runtime;
using WebhookManager.Dto;
using WebhookManager.Helpers;
using WebhookManager.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using WebhookManager.Repository;
using AutoMapper;
using WebhookManager.Services;
using WebhookManager.Config;

namespace WebhookManager.Profile.Extensions
{
    public static class ServicesExtensionsHelper
    {

        public static void ConfigureAws(this IServiceCollection services, IHostingEnvironment environment)
        {
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string key = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            string secret = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            string region = Environment.GetEnvironmentVariable("AWS_REGION");
            string api = Environment.GetEnvironmentVariable("API_NAME");

            MyAWSConfig myAWS = new MyAWSConfig()
            {
                Key = key,
                Secret = secret,
                Region = region,
                ApiName = api

            };

            if (environment.IsDevelopment())
                myAWS.AWSCredentials = new StoredProfileAWSCredentials("poc");
            else
               myAWS.AWSCredentials = new EnvironmentVariablesAWSCredentials();

            AwsFactory.AwsConfig = myAWS;
       

        }

        public static Application ConvertToModel(this ApplicationCreateRequest dto)
        {
            return new Application
            {
                Id = dto.ApplicationId,

                Description = dto.Description,
                Name = dto.Name,
                Events = dto.Events.ToList().Select(e => new Event { EventName = e.Name }).ToList() //.ToList<EventDto>().SelectMany<EventDto>(e => new Event() { Name = e.Name, Id = e.Id })
            };

        }

        public static ApplicationCreateRequest ConvertToDto(this Application hook)
        {
            return new ApplicationCreateRequest
            {
                ApplicationId = hook.Id,
                Description = hook.Description,
                Name = hook.Name,
                Events = hook.Events.ToList().Select(e => new EventDto() { Name = e.EventName }).ToList()
            };
        }


        public static IServiceCollection AddMapper(this IServiceCollection services)
        {
            services.AddAutoMapper();

            Mapper.Initialize(expression => expression.AddProfiles(Assembly.GetExecutingAssembly()));
            return services;
        }
            public static IServiceCollection AddDependenciesResolver(this IServiceCollection services)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var servicesTypes = types.Where(x => x.IsInterface)
                                        .Select(x => x.GetTypeInfo())
                                        .Where(x => x.ImplementedInterfaces.Any(y => y == typeof(IServiceBase) || y == typeof(IRepository)))
                                        .ToArray();

            foreach (var type in servicesTypes)
            {
                var obj = types.FirstOrDefault(x => x.GetTypeInfo().ImplementedInterfaces.Any(y => y == type));
                if (obj != null)
                    services.AddScoped(type, obj);
            }



            return services;

        }
        public static void AddElasticsearch(
            this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex)
                .DefaultMappingFor<Model.Profile>(m => m
                    //.Ignore(p => p.IsPublished)
                    .PropertyName(p => p.Id, "id")
                );
            //.DefaultMappingFor<Comment>(m => m
            //    .Ignore(c => c.Email)
            //    .Ignore(c => c.IsAdmin)
            //    .PropertyName(c => c.ID, "id")
            //);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }

       

       
        public static async Task ForEachAsync<T>(this List<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                await Task.Run(() => { action(item); }).ConfigureAwait(false);
        }

    }
}