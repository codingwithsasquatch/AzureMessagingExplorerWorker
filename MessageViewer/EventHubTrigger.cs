using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
//using Microsoft.Azure.WebJobs.Extensions.EventHubs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace AzureMessagingExplorer
{
    public static class EventHubTrigger
    {
        [FunctionName("EventHubTrigger")]
        [Disable("EventHubTrigger_Disabled")]
        public static async Task<bool> Run( [EventHubTrigger("samples-workitems", Connection = "EventHubConnectionAppSetting")] string myEventHubMessage,
                                [SignalR(HubName = "broadcast")]IAsyncCollector<SignalRMessage> signalRMessages, TraceWriter log)
        {
            await signalRMessages.AddAsync(new SignalRMessage()
            {
                Target = "notify",
                Arguments = new object[] { myEventHubMessage }
            });

            return true;
        }
    }
}
