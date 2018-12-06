using AzureSearch.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class Conditions
    {
        public static async Task<List<SuggestionResponse>> GetSuggestions(string azureSearchTerm)
        {
            SearchParameters searchParameters = new SearchParameters
            {
                Select = new[]
                {
                    "id","isPrimaryCare","condition"
                },
                IncludeTotalResultCount = true,
                QueryType = QueryType.Simple,
                SearchFields = new string[] { "condition" },
                Top = 6
            };

            ISearchIndexClient indexClient = AzureSearchConnectionCache.GetIndexClient(AzureSearchConnectionCache.IndexNames.conditions);
            DocumentSearchResult<ConditionIndexDataStructure> searchResults = await indexClient.Documents.SearchAsync<ConditionIndexDataStructure>(azureSearchTerm, searchParameters);
            List<SearchResult<ConditionIndexDataStructure>> results = searchResults.Results.ToList();

            List<SuggestionResponse> suggestions = new List<SuggestionResponse>();
            foreach (ConditionIndexDataStructure c in results.Select(r => r.Document))
            {
                suggestions.Add(new SuggestionResponse
                {
                    Category = "Condition",
                    SubCategory = new SubCategory
                    {
                        Code = (c.isPrimaryCare ? "PrimaryCare" : "Specialist"),
                        Text = (c.isPrimaryCare ? "Primary Care Providers" : "Specialists")
                    },
                    Suggestion = c.condition
                });
            }

            return suggestions;
        }
    }
}
