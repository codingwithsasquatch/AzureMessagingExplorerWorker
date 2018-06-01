using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AzureMessagingExplorer
{
    public static class StorageQueueTrigger
    {
        [FunctionName("StorageQueueTrigger")]
        [Disable("StorageQueueTrigger_Disabled")]
        public static void Run([QueueTrigger("queuename", Connection = "StorageQueueConnectionAppSetting")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
