using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureSearch.Common
{
    public class SubCategory
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
    public class SuggestionResponse
    {
        /// <summary>
        /// Corresponds to the category headings such as Names and Specialties
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }
        /// <summary>
        /// In the case of specialties, this would correspond to the alias (for the most part).
        /// </summary>
        [JsonProperty("suggestion")]
        public string Suggestion { get; set; }
        /// <summary>
        /// In the case of specialties, this would correspond to the specialty itself.
        /// </summary>
        [JsonProperty("subcategory")]
        public SubCategory SubCategory { get; set; }
    }
}
