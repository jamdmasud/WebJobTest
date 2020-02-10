using System;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace WebJobTest
{
    public class CustomJobActivator : IJobActivator
    {
        private readonly IServiceProvider serviceProvider;
        public CustomJobActivator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public T CreateInstance<T>()
        {
            return serviceProvider.GetService<T>();
        }
    }
}