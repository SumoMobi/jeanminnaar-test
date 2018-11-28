using AzureSearch.Common;
using Microsoft.Azure;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class AzureSearchProviderQueryResponse
    {
        public string id { get; set; }
        public int searchRank { get; set; }
    }
    public class Filter
    {

        /// <summary>
        /// These are the names like condition, specialty, acceptNewPatients and so on.
        /// </summary>
        public string FilterName { get; set; }

        /// <summary>
        /// In the case of a suggestion filter, there is just one value.  But in the case of say languages, there could be multiple values.  These values to be treated as AND requests.
        /// </summary>
        public List<string> Values { get; set; }
    }
    public class Providers
    {
        public static async Task<List<AzureSearchProviderQueryResponse>> GetProviders(int skip, int take, string universal, List<Filter> filters)
        {
            SearchServiceClient serviceClient = new SearchServiceClient(
                CloudConfigurationManager.GetSetting("serviceName"), new SearchCredentials(CloudConfigurationManager.GetSetting("apiKey")));

            List<string> facets = new List<string>()
            {
                "agesSeen",
                "acceptedInsurances",
                "acceptNewPatients",
                "isMale",
                "providerType",
                "languages",
                "networkAffiliations"
            };
            List<string> searchFields = null;
            string search = "*";
            SearchMode searchMode = SearchMode.All;
            string queryType = "simple";
            if (universal != null)
            {
                searchFields = new List<string>();
                searchFields.Add("acceptedInsurancesLower");
                searchFields.Add("firstAndLastNameLower");
                searchFields.Add("conditionsLower");
                searchFields.Add("specialtiesLower");
                searchFields.Add("cities");
                searchFields.Add("zipCodes");
                search = universal; //wild cards?
            }
            string filter = null;
            if (filters.Count > 0)
            {
                queryType = "full";
                filter = string.Empty;
                foreach (Filter f in filters)
                {
                    string quote = string.Empty;
                    if (f.FilterName.EmCompareIgnoreCase("isMale") || f.FilterName.EmCompareIgnoreCase("acceptNewPatients"))
                    {
                        quote = "'";
                    }
                    foreach (string val in f.Values)
                    {
                        filter += $"({f.FilterName} eq {quote}{val}{quote}) and ";
                    }
                }
                filter = filter.Substring(0, filter.Length - 5);    //Chop off the last AND.
            }

            SearchParameters searchParameters = new SearchParameters
            {
                Facets = facets.ToArray(),
                Filter = filter,
                IncludeTotalResultCount = true, 
                OrderBy = new List<string>() { "searchRank", "randomNumber" }, //What about relevance/score?
                QueryType = QueryType.Simple,
                SearchFields = searchFields,
                SearchMode = searchMode,
                Select = new[]
                {
                    "id","searchRank"
                },
                Skip = skip,
                Top = take
            };

            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("providers");
            DocumentSearchResult<AzureSearchProviderQueryResponse> searchResults = await indexClient.Documents.SearchAsync<AzureSearchProviderQueryResponse>(search, searchParameters);
            List<SearchResult<AzureSearchProviderQueryResponse>> results = searchResults.Results.ToList();

            return results.Select(r => r.Document).ToList();
        }
    }
}
