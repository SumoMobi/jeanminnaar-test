using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AzureSearch.RefreshComplete
{
    public static class Test01_Func
    {
        [FunctionName("Test01")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/test01")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Test01 function processed a request.");

            return (ActionResult)new OkObjectResult($"Test01");
        }
        [FunctionName("Test02")]
        public static async Task<IActionResult> Test02(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/test02")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Test02 function processed a request.");

            return (ActionResult)new OkObjectResult($"Test02");
        }
        [FunctionName("RefreshComplete2")]
        //public static void Run([EventHubTrigger("refresh-complete", Connection = "refreshCompleteConnectionString", ConsumerGroup = "non-prod")]
        //    string myEventHubMessage,
        //    ILogger log)
        //public static void Run([EventHubTrigger("refresh-complete", Connection = "refreshCompleteConnectionString", ConsumerGroup = "non-prod")]
        //    EventData myEventHubMessage,
        //    ILogger log)
        public static void Run2([EventHubTrigger("refresh-complete", Connection = "refreshCompleteConnectionString", ConsumerGroup = "%consumerGroup%")]
            EventData[] myEventHubMessages,
            ILogger log)
        {
            //Microsoft.Azure.WebJobs.Host.Bindings.ValueBindingContext.
            //Attribute.GetCustomAttribute(typeof(EventHubTriggerAttribute).GetEvent(""), typeof(EventHubTriggerAttribute));
            log.LogInformation($"RefreshComplete Run2 processed a set of messages.  Count: {myEventHubMessages.Length}");
        }
    }
}
