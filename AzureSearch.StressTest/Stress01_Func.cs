using AzureSearch.Api;
using AzureSearch.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureSearch.StressTest
{
    public class PerformanceResult
    {
        public double SuggestionTime { get; set; }
        public double SearchTime { get; set; }
    }
    public static class Stress01_Func
    {
        public static List<SearchStat> searchStats = new List<SearchStat>()
        {
            new SearchStat
            {
                NumberOfRequests = 10,
                specialtySearch = "Family Medicine",
                suggestionSearch = "family*"
            },
            new SearchStat
            {
                NumberOfRequests = 9,
                specialtySearch = "Internal Medicine",
                suggestionSearch = "Internal*"
            },
            new SearchStat
            {
                NumberOfRequests = 9,
                specialtySearch = "Gynecology",
                suggestionSearch = "Obstetr*"
            },
            new SearchStat
            {
                NumberOfRequests = 4,
                specialtySearch = "Cardiology",
                suggestionSearch = "cardiology*"
            },
            new SearchStat
            {
                NumberOfRequests = 4,
                specialtySearch = "Orthopedic Surgery",
                suggestionSearch = "Ortho*"
            },
            new SearchStat
            {
                NumberOfRequests = 4,
                specialtySearch = "Gastroenterology",
                suggestionSearch = "Gastro*"
            },
            new SearchStat
            {
                NumberOfRequests = 4,
                specialtySearch = "Pediatric Neurology",
                suggestionSearch = "neurology*"
            },
            new SearchStat
            {
                NumberOfRequests = 3,
                specialtySearch = "Dermatology",
                suggestionSearch = "dermatology*"
            },
            new SearchStat
            {
                NumberOfRequests = 3,
                specialtySearch = "Pediatrics",
                suggestionSearch = "pediatrics*"
            },
            new SearchStat
            {
                NumberOfRequests = 3,
                specialtySearch = "Psychiatry",
                suggestionSearch = "Psychiatr*"
            },
        };
        public static DateTime stopAfterThisDt;

        [FunctionName("Stress01")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/stresstest/numberOfUsers/{users}/secondsToRun/{seconds}")] HttpRequest req,
            int users,
            int seconds,
            ILogger log)
        {
            try
            {
                List<SearchStat> searchList = GetSearchList();
                List<Task> tasks = new List<Task>(users);
                Random random = new Random(DateTime.Now.Millisecond);
                ConcurrentBag<PerformanceResult> performanceResults = new ConcurrentBag<PerformanceResult>();

                stopAfterThisDt = DateTime.Now + new TimeSpan(0, 0, seconds);

                for (int i = 0; i < users; i++)
                {
                    //Randomly pick a search criteria entry for the user.
                    int r = random.Next(0, searchList.Count - 1);
                    tasks.Add(SimulateUser(searchList[r], performanceResults));
                }
                Task.WaitAll(tasks.ToArray());
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(performanceResults), System.Text.Encoding.UTF8, "application/json")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent($"{ex.Message}.  {ex.StackTrace}", Encoding.UTF8, "text/plain")
                };
            }
        }

        private static async Task SimulateUser(SearchStat searchStat, ConcurrentBag<PerformanceResult> performanceResults)
        {
            await Task.Run(() =>
            {
                Random r = new Random(DateTime.Now.Millisecond);
                List<Filter> filters = new List<Filter>()
                {
                    new Filter
                    {
                        AzureIndexFieldName = "specialties",
                        Values = new List<string>()
                        {
                            searchStat.specialtySearch
                        }
                    }
                };

                while (true)
                {
                    PerformanceResult pr = new PerformanceResult();

                    //Suggestions
                    DateTime startDt = DateTime.Now;
                    GetSuggestions(searchStat.suggestionSearch);
                    pr.SuggestionTime = (DateTime.Now - startDt).TotalMilliseconds;
                    Thread.Sleep(r.Next(1000, 2000));

                    //Providers
                    startDt = DateTime.Now;
                    Task<DocumentSearchResult<AzureSearchProviderRequestedFields>> task = Providers.GetProviders(0, 25, null, filters, false, false);
                    task.Wait();
                    pr.SearchTime = (DateTime.Now - startDt).TotalMilliseconds;
                    Thread.Sleep(r.Next(1000, 2000));

                    performanceResults.Add(pr);

                    if (DateTime.Now > stopAfterThisDt)
                    {
                        break;
                    }
                }
            });
        }
        //private static void GetProviders(List<Filter> filters)
        //{
        //    Task<DocumentSearchResult<AzureSearchProviderRequestedFields>> task = Providers.GetProviders(0, 25, null, filters, false, false);
        //    task.Wait();
        //}
        private static void GetSuggestions(string searchTerm)
        {
            List<Task<List<SuggestionResponse>>> tasks = new List<Task<List<SuggestionResponse>>>(4);
            tasks.Add(Conditions.GetSuggestions(searchTerm));   //13K condition entries. (1.6MB)  Kick it off first.
            tasks.Add(Names.GetSuggestions(searchTerm));        //4K name entries. (0.6MB)
            tasks.Add(Specialties.GetSuggestions(searchTerm));  //2K specialty entries. (0.4MB)
            tasks.Add(Insurances.GetSuggestions(searchTerm));   //0.1K insurance entries. (0.1MB)

            Task.WaitAll(tasks.ToArray());

        }
        private static List<SearchStat> GetSearchList()
        {
            List<SearchStat> searchList = new List<SearchStat>();
            foreach (SearchStat ss in searchStats)
            {
                for (int i = 0; i < ss.NumberOfRequests; i++)
                {
                    searchList.Add(new SearchStat
                    {
                        NumberOfRequests = -1,
                        specialtySearch = ss.specialtySearch,
                        suggestionSearch = ss.suggestionSearch
                    });
                }
            }
            return searchList;
        }
        public class SearchStat
        {
            public string suggestionSearch { get; set; }
            public string specialtySearch { get; set; }
            public int NumberOfRequests { get; set; }
        }
        //public class RandomSearch
        //{
        //    public string suggestionSearch { get; set; }
        //    public string specialtySearch { get; set; }
        //    public int randomNumber { get; set; }
        //}
    }
}
