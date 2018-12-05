using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class AzureSearchProviderRequestedFields
    {
        public string id { get; set; }
        public int searchRank { get; set; }
    }
    public class Filter
    {

        /// <summary>
        /// These are the names like condition, specialty, acceptNewPatients and so on.
        /// </summary>
        public string AzureIndexFieldName { get; set; }

        /// <summary>
        /// In the case of a suggestion filter, there is just one value.  But in the case of say languages, there could be multiple values.  These values to be treated as AND requests.
        /// </summary>
        public List<string> Values { get; set; }
    }
    public enum AzureIndexFieldTypes { text, collection, boolean };
    public class FilterMapInfo
    {
        /// <summary>
        /// This is like "Condition"
        /// </summary>
        public string FilterName { get; set; }
        /// <summary>
        /// The field name in the Azure Search index
        /// </summary>
        public string AzureIndexFieldName { get; set; }
        /// <summary>
        /// Is the field a collection type in Azure Search
        /// </summary>
        public AzureIndexFieldTypes AzureIndexFieldType { get; set; }
        /// <summary>
        /// A set of filter requests can only originate from a user clicking on a suggestion.
        /// </summary>
        public bool IsSuggestion { get; set; }
    }
    public class Providers
    {
        public static List<FilterMapInfo> filterMapInfoList = new List<FilterMapInfo>()
        {
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "conditions",
                FilterName = "condition_specialist",
                IsSuggestion = true
            },
            new FilterMapInfo
            {   //Need to convert the search term/value entered to a true value.  TODO
                AzureIndexFieldType = AzureIndexFieldTypes.boolean,
                AzureIndexFieldName = "isPrimaryCare",
                FilterName = "condition_primarycare",
                IsSuggestion = true
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.text,
                AzureIndexFieldName = "firstAndLastName",
                FilterName = "name",
                IsSuggestion = true
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "specialties",
                FilterName = "specialty",
                IsSuggestion = true
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "acceptedInsurances",
                FilterName = "insurance",
                IsSuggestion = true
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "agesSeen",
                FilterName = "agesseen",
                IsSuggestion = false
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "acceptedInsurances",
                FilterName = "acceptedinsurances",
                IsSuggestion = false
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.boolean,
                AzureIndexFieldName = "acceptNewPatients",
                FilterName = "acceptnewpatients",
                IsSuggestion = false
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.boolean,
                AzureIndexFieldName = "isMale",
                FilterName = "ismale",
                IsSuggestion = false
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "providerType",
                FilterName = "providertypes",
                IsSuggestion = false
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "languages",
                FilterName = "languages",
                IsSuggestion = false
            },
            new FilterMapInfo
            {
                AzureIndexFieldType = AzureIndexFieldTypes.collection,
                AzureIndexFieldName = "networkAffiliations",
                FilterName = "networkaffiliations",
                IsSuggestion = false
            },
        };

        private static List<String> universalSearchFields = new List<string>()
        {
            "acceptedInsurancesLower",
            "firstAndLastNameLower",
            "conditionsLower",
            "specialtiesLower",
            "cities",
            "zipCodes",
        };

        public static SearchParameters BuildAzureSearchParameters(int skip, int take, string universal, List<Filter> filters)
        {
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
            QueryType queryType = QueryType.Simple;
            if (universal != null)
            {
                searchFields = universalSearchFields;
                search = universal; //wild cards?
            }
            string filter = null;
            if (filters.Count > 0)
            {
                queryType = QueryType.Full;
                filter = string.Empty;
                foreach (Filter f in filters)
                {
                    //Find filter entry in the filter map info list.
                    FilterMapInfo fmInfo = filterMapInfoList.Single(m => m.AzureIndexFieldName == f.AzureIndexFieldName);
                    foreach (string val in f.Values)
                    {
                        switch (fmInfo.AzureIndexFieldType)
                        {
                            case AzureIndexFieldTypes.collection:
                                filter += $"({fmInfo.AzureIndexFieldName}/any(i: i eq '{val}')) and ";
                                break;
                            case AzureIndexFieldTypes.boolean:
                                filter += $"({f.AzureIndexFieldName} eq {val}) and ";
                                break;
                            case AzureIndexFieldTypes.text:
                                filter += $"({f.AzureIndexFieldName} eq '{val}') and ";
                                break;
                                //TODO When adding lat/lon/radius, will need to add more types.  The radius searches goes off elsewhere first anyhow.
                        }
                    }
                }
                filter = filter.Substring(0, filter.Length - 5);    //Chop off the last 'and'.
            }

            SearchParameters searchParameters = new SearchParameters
            {
                Facets = facets.ToArray(),
                Filter = filter,
                IncludeTotalResultCount = true,
                OrderBy = new List<string>() { "searchRank", "randomNumber" }, //What about relevance/score?
                QueryType = queryType,
                SearchFields = searchFields,
                SearchMode = searchMode,
                Select = new[]
                {
                    "id","searchRank"
                },
                Skip = skip,
                Top = take
            };
            return searchParameters;
        }
        public static async Task<DocumentSearchResult<AzureSearchProviderRequestedFields>> GetProviders(int skip, int take, string universal, List<Filter> filters)
        {
            SearchServiceClient serviceClient = new SearchServiceClient(
                Environment.GetEnvironmentVariable("serviceName", EnvironmentVariableTarget.Process), 
                new SearchCredentials
                    (Environment.GetEnvironmentVariable("apiKey", EnvironmentVariableTarget.Process)
                ));

            SearchParameters searchParameters = BuildAzureSearchParameters(skip, take, universal, filters);

            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("providers");
            DocumentSearchResult<AzureSearchProviderRequestedFields> searchResults = 
                await indexClient.Documents.SearchAsync<AzureSearchProviderRequestedFields>(universal, searchParameters);
            //List<SearchResult<AzureSearchProviderQueryResponse>> results = searchResults.Results.ToList();

            return searchResults;
            //results.Select(r => r.Document).ToList();
        }
    }
}
