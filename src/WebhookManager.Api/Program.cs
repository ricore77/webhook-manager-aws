﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebhookManager.Profile

{
    public class Program 
    {
        public static void Main(string[] args)
        {
      
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
           
                .ConfigureAppConfiguration(
                    (builderContext, config) =>
                    {
                        IHostingEnvironment env = builderContext.HostingEnvironment;
                        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                    }
                )
                .ConfigureLogging(
                    (hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.ClearProviders();
                        logging.AddConsole();

                    }
                )
                .Build();
    }
}
