using Amazon.DynamoDBv2;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebhookManager.Api.Extensions;
using WebhookManager.Api.Helpers;
using WebhookManager.Dto;
using WebhookManager.Model;
using WebhookManager.Profile.Extensions;
using WebhookManager.Repository;
using WebhookManager.Services;


namespace WebhookManager.Profile
{
    public class Startup
    {
        private  IHostingEnvironment Environment { get; set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options =>
            {
                options.InputFormatters.Insert(0, new RawJsonBodyInputFormatter());
                options.AddErrorHandling(new WebhookManagerExceptionFilter());
            });

            
            services.ConfigureAws(Environment);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Webhook Manager API", Version = "v1" });
            });
            services.AddHttpClient();

           
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddElasticsearch(Configuration);
      
            
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

            services.AddDependenciesResolver();
            services.AddMapper();
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webhook Manager API");
            });

           


            app.UseMvc();
        }
    }

    
}
