using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure;
using AzureSearch.Common;
using System.Collections.Generic;
using System;

namespace AzureSearch.Api
{
    public static class Suggestions
    {
        [FunctionName("Suggestions_Any")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/suggestions/searchTerms/{searchTerms}")]HttpRequestMessage req,
            string searchTerms,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            DateTime startDt = DateTime.Now;

            SearchServiceClient serviceClient = new SearchServiceClient(
                CloudConfigurationManager.GetSetting("serviceName"), new SearchCredentials(CloudConfigurationManager.GetSetting("apiKey")));

            Task<List<SuggestionResponse>> specialtiesTask = Specialties.GetSuggestions(searchTerms, serviceClient);

            Task.WaitAll(new Task[] { specialtiesTask });

            HttpResponseMessage response = new HttpResponseMessage
            {
                //Content = new StringContent($"{conditionsTask.Result.Count} conditions, {specialtiesTask.Result.Count}, insurances {insurancesTask.Result.Count}, names {namesTask.Result.Count}" +
                //    $", {(DateTime.Now - startDt).TotalMilliseconds} milliseconds")
                Content = new StringContent($"{specialtiesTask.Result.Count} specialties, {(DateTime.Now - startDt).TotalMilliseconds} milliseconds")
            };

            response.Headers.Add("x-elapsed-time", (DateTime.Now - startDt).TotalMilliseconds.ToString());

            //The suggestion response is an array of the following:
            //{
            //    "category": "Condition",
            //    "suggestion": "diabetes insipidus",
            //    "subcategory": {
            //        "code": "Specialist",
            //        "text": "Specialists"
            //    }
            //}

            return response;
        }
    }
}
