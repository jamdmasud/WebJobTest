using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WebJobTest
{
    public class CustomQueueProcessorFactory : IQueueProcessorFactory
    {
        protected IConfiguration Configuration { get; }

        public CustomQueueProcessorFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public QueueProcessor Create(QueueProcessorFactoryContext context)
        {
            return new CustomQueueProcessor(context, Configuration);
        }

        private class CustomQueueProcessor : QueueProcessor
        {
            private IConfiguration Configuration { get; }
            public CustomQueueProcessor(QueueProcessorFactoryContext context, IConfiguration configuration)
                : base(context)
            {
                Configuration = configuration;
            }

            protected override async Task ReleaseMessageAsync(CloudQueueMessage message, FunctionResult result, TimeSpan visibilityTimeout, CancellationToken cancellationToken)
            {
                if (int.TryParse(Configuration["Storage:MessageVisibilityTimeout"], out var timeout))
                    visibilityTimeout = TimeSpan.FromSeconds(timeout);

                await base.ReleaseMessageAsync(message, result, visibilityTimeout, cancellationToken);
            }
        }
    }
}