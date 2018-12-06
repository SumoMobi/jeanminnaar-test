using AzureSearch.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class Names
    {
        public static async Task<List<SuggestionResponse>> GetSuggestions(string azureSearchTerm)
        {
            SearchParameters searchParameters = new SearchParameters
            {
                Select = new[]
                {
                    "id","firstAndLastName"
                },
                IncludeTotalResultCount = true,
                QueryType = QueryType.Simple,
                SearchFields = new string[] { "firstAndLastName" },
                Top = 5
            };

            ISearchIndexClient indexClient = AzureSearchConnectionCache.GetIndexClient(AzureSearchConnectionCache.IndexNames.names);
            DocumentSearchResult<NameIndexDataStructure> searchResults = await indexClient.Documents.SearchAsync<NameIndexDataStructure>(azureSearchTerm, searchParameters);
            List<SearchResult<NameIndexDataStructure>> results = searchResults.Results.ToList();

            List<SuggestionResponse> suggestions = new List<SuggestionResponse>();
            foreach (NameIndexDataStructure c in results.Select(r => r.Document))
            {
                suggestions.Add(new SuggestionResponse
                {
                    Category = "Name",
                    SubCategory = new SubCategory
                    {
                        Code = null,
                        Text = null
                    },
                    Suggestion = c.firstAndLastName
                });
            }

            return suggestions;
        }
    }
}
