using AzureSearch.Common;
using Microsoft.Azure;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureSearch.Api
{
    public class Specialties
    {
        public static async Task<List<SuggestionResponse>> GetSuggestions(string searchTerms, SearchServiceClient serviceClient)
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
                Top = 6
            };

            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("specialties");
            DocumentSearchResult<SpecialtyIndexDataStructure> suggestions = await indexClient.Documents.SearchAsync<SpecialtyIndexDataStructure>(searchTerms + "*", searchParameters);
            List<SearchResult<SpecialtyIndexDataStructure>> results = suggestions.Results.ToList();

            List<SuggestionResponse> suggestionList = new List<SuggestionResponse>();
            foreach(SpecialtyIndexDataStructure s in results.Select(r => r.Document))
            {
                suggestionList.Add(new SuggestionResponse
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

            return suggestionList;
        }
    }
}
