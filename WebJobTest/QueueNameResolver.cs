using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace WebJobTest
{
    public class QueueNameResolver : INameResolver
    {
        protected IConfiguration Configuration { get; }
        public QueueNameResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string Resolve(string name)
        {
            var queueName = Configuration[$"Queues:{name}"];
            return queueName;
        }
    }
}
