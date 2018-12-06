using AzureSearch.Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class Specialties
    {
        public static async Task<List<SuggestionResponse>> GetSuggestions(string azureSearchTerm)
        {
            SearchParameters searchParameters = new SearchParameters
            {
                Select = new[]
                {
                    "id","specialty","alias"
                },
                IncludeTotalResultCount = true,
                QueryType = QueryType.Simple,
                SearchFields = new string[] { "alias" }, 
                Top = 5
            };

            ISearchIndexClient indexClient = AzureSearchConnectionCache.GetIndexClient(AzureSearchConnectionCache.IndexNames.specialties);
            DocumentSearchResult<SpecialtyIndexDataStructure> searchResults = await indexClient.Documents.SearchAsync<SpecialtyIndexDataStructure>(azureSearchTerm, searchParameters);
            List<SearchResult<SpecialtyIndexDataStructure>> results = searchResults.Results.ToList();
            List<SuggestionResponse> suggestions = new List<SuggestionResponse>();
            foreach(SpecialtyIndexDataStructure s in results.Select(r => r.Document))
            {
                suggestions.Add(new SuggestionResponse
                {
                    Category = "Specialty",
                    SubCategory = new SubCategory
                    {
                        Code = "",
                        Text = s.specialty  //If we have a specialty without an alias, do we do the right thing here?  TODO
                    },
                    Suggestion = s.alias
                });
            }

            return suggestions;
        }
    }
}
