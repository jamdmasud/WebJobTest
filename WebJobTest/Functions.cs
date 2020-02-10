using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebJobTest
{
    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("%TestQueue%")] string message, ILogger logger)
        {
            logger.LogInformation(message);
        }
    }
}
