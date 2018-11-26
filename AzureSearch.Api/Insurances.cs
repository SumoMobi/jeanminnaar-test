using AzureSearch.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class Insurances
    {
        public static async Task<List<SuggestionResponse>> GetSuggestions(string azureSearchTerm, SearchServiceClient serviceClient)
        {
            SearchParameters searchParameters = new SearchParameters
            {
                Select = new[]
                {
                    "id","insurance"
                },
                IncludeTotalResultCount = true,
                QueryType = QueryType.Simple,
                SearchFields = new string[] { "insurance" },
                Top = 5
            };

            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("insurances");
            DocumentSearchResult<InsuranceIndexDataStructure> searchResults = await indexClient.Documents.SearchAsync<InsuranceIndexDataStructure>(azureSearchTerm, searchParameters);
            List<SearchResult<InsuranceIndexDataStructure>> results = searchResults.Results.ToList();

            List<SuggestionResponse> suggestions = new List<SuggestionResponse>();
            foreach (InsuranceIndexDataStructure c in results.Select(r => r.Document))
            {
                suggestions.Add(new SuggestionResponse
                {
                    Category = "Insurance",
                    SubCategory = new SubCategory
                    {
                        Code = null,
                        Text = null
                    },
                    Suggestion = c.insurance
                });
            }

            return suggestions;
        }
    }
}
