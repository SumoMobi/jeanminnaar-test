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
    public static class Suggestions_Func
    {
        [FunctionName("Suggestions")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/suggestions/searchTerms/{searchTerms}")]HttpRequestMessage req,
            string searchTerms,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            DateTime startDt = DateTime.Now;

            SearchServiceClient serviceClient = new SearchServiceClient(
                CloudConfigurationManager.GetSetting("serviceName"), new SearchCredentials(CloudConfigurationManager.GetSetting("apiKey")));

            List<SuggestionResponse> suggestions = new List<SuggestionResponse>();
            HttpResponseMessage response;

            string st = searchTerms.Trim();
            if (string.IsNullOrWhiteSpace(st))
            {
                response = new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(suggestions), System.Text.Encoding.UTF8, "application/json")
                };

                response.Headers.Add("bh-dg-elapsed-time", (DateTime.Now - startDt).TotalMilliseconds.ToString());
                return response;
            }

            string[] sts = st.Split(new char[] { ' ' });
            for(int t = 0; t < sts.Length; t++)
            {
                if (sts[t].EndsWith("*") == false)
                {
                    sts[t] += "*"; 
                }
            }
            string azureSearchTerm = string.Join("+", sts);

            List<Task<List<SuggestionResponse>>> tasks = new List<Task<List<SuggestionResponse>>>();
            tasks.Add(Conditions.GetSuggestions(azureSearchTerm, serviceClient));   //13K condition entries. (1.6MB)  Kick it off first.
            tasks.Add(Names.GetSuggestions(azureSearchTerm, serviceClient));        //4K name entries. (0.6MB)
            tasks.Add(Specialties.GetSuggestions(azureSearchTerm, serviceClient));  //2K specialty entries. (0.4MB)
            tasks.Add(Insurances.GetSuggestions(azureSearchTerm, serviceClient));   //0.1K insurance entries. (0.1MB)

            Task.WaitAll(tasks.ToArray());

            foreach(Task<List<SuggestionResponse>> t in tasks)
            {
                suggestions.AddRange(t.Result);
            }
            response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(suggestions), System.Text.Encoding.UTF8, "application/json")
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
