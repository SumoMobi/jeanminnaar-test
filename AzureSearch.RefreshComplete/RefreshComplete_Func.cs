using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace AzureSearch.RefreshComplete
{
    public static class RefreshComplete_Func
    {
        [FunctionName("RefreshComplete")]
        //public static void Run([EventHubTrigger("refresh-complete", Connection = "refreshCompleteConnectionString", ConsumerGroup = "non-prod")]
        //    string myEventHubMessage,
        //    ILogger log)
        //public static void Run([EventHubTrigger("refresh-complete", Connection = "refreshCompleteConnectionString", ConsumerGroup = "non-prod")]
        //    EventData myEventHubMessage,
        //    ILogger log)
        public static void Run([EventHubTrigger("refresh-complete", Connection = "refreshCompleteConnectionString", ConsumerGroup = "%consumerGroup%")]
            EventData[] myEventHubMessages,
            ILogger log)
        {
            //Microsoft.Azure.WebJobs.Host.Bindings.ValueBindingContext.
            //Attribute.GetCustomAttribute(typeof(EventHubTriggerAttribute).GetEvent(""), typeof(EventHubTriggerAttribute));
            log.LogInformation($"RefreshComplete Event Hub trigger function, using consumer group, processed a set of messages.  Count: {myEventHubMessages.Length}");
        }
    }
}
