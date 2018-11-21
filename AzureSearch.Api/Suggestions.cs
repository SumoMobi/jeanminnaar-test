using AzureSearch.Common;
using Microsoft.Azure;
using Microsoft.Azure.Search;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public static class Suggestions
    {
        [FunctionName("Suggestions_Any")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/suggestions/searchTerms/{searchTerms}")]HttpRequestMessage req,
            string searchTerms,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            DateTime startDt = DateTime.Now;

            SearchServiceClient serviceClient = new SearchServiceClient(
                CloudConfigurationManager.GetSetting("serviceName"), new SearchCredentials(CloudConfigurationManager.GetSetting("apiKey")));

            List<Task<List<SuggestionResponse>>> tasks = new List<Task<List<SuggestionResponse>>>();
            tasks.Add(Specialties.GetSuggestions(searchTerms, serviceClient));
            tasks.Add(Conditions.GetSuggestions(searchTerms, serviceClient));
            tasks.Add(Insurances.GetSuggestions(searchTerms, serviceClient));
            tasks.Add(Names.GetSuggestions(searchTerms, serviceClient));

            Task.WaitAll(tasks.ToArray());

            List<SuggestionResponse> suggestions = new List<SuggestionResponse>();
            foreach(Task<List<SuggestionResponse>> t in tasks)
            {
                suggestions.AddRange(t.Result);
            }
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(suggestions))
            };

            response.Headers.Add("bh-dg-elapsed-time", (DateTime.Now - startDt).TotalMilliseconds.ToString());

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
