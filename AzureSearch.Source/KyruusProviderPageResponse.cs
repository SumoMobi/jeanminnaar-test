using AzureSearch.Common;
using System.Collections.Generic;

namespace AzureSearch.Source
{
    public class KyruusProviderPageResponse
    {
        public object alerts { get; set; } 
        public int availability_format { get; set; } 
        public object[] facets { get; set; }
        public object geocoded_location { get; set; }
        public object interpretation { get; set; } 
        public List<RelaxedProvider> providers { get; set; }
    }

}
