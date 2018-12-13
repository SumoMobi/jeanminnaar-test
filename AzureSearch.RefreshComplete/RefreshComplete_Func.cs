using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureSearch.RefreshComplete
{
    public static class RefreshComplete_Func
    {
        [FunctionName("RefreshComplete")]
        public static void Run([EventHubTrigger("refresh-complete", Connection = "refreshCompleteConnectionString", ConsumerGroup = "non-prod")]
            string myEventHubMessage, 
            ILogger log)
        {
            log.LogInformation($"RefreshComplete Event Hub trigger function processed a message: {myEventHubMessage}");
        }
    }
}
