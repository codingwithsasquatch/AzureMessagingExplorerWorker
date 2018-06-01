
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace AzureMessagingExplorer
{
    public static class HttpTrigger
    {
        [FunctionName("HttpTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req, 
                                                    [SignalR(HubName = "broadcast")]IAsyncCollector<SignalRMessage> signalRMessages, 
                                                    TraceWriter log)
        {
            var requestContent = await req.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(requestContent))
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "Please pass a payload to broadcast in the request body.");
            }

            log.Info($"Message with payload {requestContent}");

            await signalRMessages.AddAsync(new SignalRMessage()
            {
                Target = "notify",
                Arguments = new object[] { requestContent }
            });

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
