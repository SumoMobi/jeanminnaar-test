
using System.Collections.Generic;

namespace TypeAheadOrdering.DataModels
{
    public class TypeAheadItem
    {
        public string Category { get; set; }
        public string Suggestion { get; set; }
    }

    public class SuggestionPlusWeight
    {
        public string Suggestion { get; set; }
        public int Weight { get; set; }
    }

    //public class SearchWordAndWeight
    //{
    //    public string searchWord { get; set; }
    //    public int weight { get; set; }
    //}
}