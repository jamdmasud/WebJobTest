using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs.Host.Queues;
using Microsoft.Azure.WebJobs;

namespace WebJobTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
            });

            builder.ConfigureLogging((context, logging) =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                var appInsightsKey = context.Configuration["AppInsights:InstrumentationKey"];
                if (!string.IsNullOrEmpty(appInsightsKey))
                {
                    // This uses the options callback to explicitly set the instrumentation key.
                    logging.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = appInsightsKey);
                }

                logging.AddConsole();
            });

            builder.ConfigureServices((context, services) =>
            {
                services.Configure<QueuesOptions>(o =>
                {
                    o.BatchSize = int.Parse(context.Configuration["Storage:BatchSize"]);
                    o.MaxPollingInterval =
                        TimeSpan.FromSeconds(
                            int.Parse(context.Configuration["Storage:MaxPollingInterval"]));
                    o.MaxDequeueCount = int.Parse(context.Configuration["Storage:MaxDequeueCount"]);
                    ConfigureServices(context.Configuration, services);
                });


                var customQueueProcessorFactory = new CustomQueueProcessorFactory(context.Configuration);
                services.AddSingleton<IQueueProcessorFactory>(customQueueProcessorFactory);
                services.AddSingleton<INameResolver>(new QueueNameResolver(context.Configuration));
                services.AddSingleton<IJobActivator, CustomJobActivator>();

                services.AddScoped<Functions>();
            });

            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }


        static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            // Configuration
            services.Configure<TestServiceConfig>(configuration.GetSection("TestService"));


            // WebJob
            services.AddTransient<Functions, Functions>();
        }
    }
}
