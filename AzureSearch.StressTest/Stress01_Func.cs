using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AzureSearch.Api;
using AzureSearch.Common;

namespace AzureSearch.StressTest
{
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

        [FunctionName("Stress01")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/stresstest/threads/{threads}")] HttpRequest req,
            int threads,
            ILogger log)
        {
            try
            {
                DateTime startDt = DateTime.Now;
                List<SearchStat> searchList = GetSearchList();
                List<Task> tasks = new List<Task>(threads);
                Random random = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i < threads; i++)
                {
                    //Randomly pick a search criteria entry.
                    int r = random.Next(0, searchList.Count - 1);
                    tasks.Add(RunThread(searchList[r]));
                }
                Task.WaitAll(tasks.ToArray());
                return new HttpResponseMessage
                {
                    Content = new StringContent($"{threads} threads.  {(DateTime.Now - startDt).TotalMilliseconds.ToString()} milliseconds", Encoding.UTF8, "text/plain")
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

        private static async Task RunThread(SearchStat searchStat)
        {
            await Task.Run(() =>
            {
                GetSuggestions(searchStat.suggestionSearch);
            });
        }
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
