using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Collections.Generic;
using System.Text;
using System;
using AzureMessagingExplorer.Models;
using AzureMessagingExplorer.MessageEngine;
using System.Linq;

namespace AzureMessagingExplorer
{
    public static class SendStorageQueueMessageBatch
    {
        [FunctionName("Job_SendStorageQueueMessageBatch")]
        public static async Task<string> Run([ActivityTrigger] StorageQueueJobProperties sqJobProperties, TraceWriter log)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(sqJobProperties.ConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(sqJobProperties.Queue);
            await queue.CreateIfNotExistsAsync();

            int secondsPerBatch = Convert.ToInt16(Environment.GetEnvironmentVariable("secondsPerBatch"));
            int messagesInBatch = sqJobProperties.Frequency * secondsPerBatch;
            IEnumerable<string> messages = Messages.CreateMessages(messagesInBatch, sqJobProperties.MessageScheme);

            try
            {
                foreach (string message in messages) {
                    await queue.AddMessageAsync(new CloudQueueMessage(message));
                }
                //var sbMessages = (List<CloudQueueMessage>) messages.Select(m => new CloudQueueMessage(m));
            }
            catch (Exception exception)
            {
                log.Info($"Exception: {exception.Message}");
            }

            log.Info($"sent batch of {messages.Count()} messages");
            return $"finished sending {messages.Count()} to {sqJobProperties.Queue}!";
        }
    }
}
