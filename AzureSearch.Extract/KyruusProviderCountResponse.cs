namespace AzureSearch.Extract.Count
{
    public class KyruusProviderCountResponse
    {
        public object alerts { get; set; }
        public int availability_format { get; set; }
        public Facet[] facets { get; set; }
        public Geocoded_Location geocoded_location { get; set; }
        public Interpretation interpretation { get; set; }
        public object[] providers { get; set; }
        public Suggestions suggestions { get; set; }
        public int total_providers { get; set; }
        public object[] warnings { get; set; }
    }

    public class Geocoded_Location
    {
    }

    public class Interpretation
    {
    }

    public class Suggestions
    {
    }

    public class Facet
    {
        public string field { get; set; }
        public int missing { get; set; }
        public int other { get; set; }
        public Term[] terms { get; set; }
        public int total { get; set; }
    }

    public class Term
    {
        public int count { get; set; }
        public string value { get; set; }
    }
}
