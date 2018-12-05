namespace AzureSearch.Api.ResponseStructures.Current
{
    public class Rootobject
    {
        public string odatacontext { get; set; }
        public int odatacount { get; set; }
        public SearchFacets searchfacets { get; set; }
        public SearchNextpageparameters searchnextPageParameters { get; set; }
        public Value[] value { get; set; }
        public string odatanextLink { get; set; }
        public string seed { get; set; }
        public string echoBackInQueryString { get; set; }
    }

    public class SearchFacets
    {
        public string acceptedInsurancesodatatype { get; set; }
        public Acceptedinsurance[] acceptedInsurances { get; set; }
        public string acceptNewPatientsodatatype { get; set; }
        public Acceptnewpatient[] acceptNewPatients { get; set; }
        public string isMaleodatatype { get; set; }
        public Ismale[] isMale { get; set; }
        public string languagesodatatype { get; set; }
        public Language[] languages { get; set; }
    }

    public class Acceptedinsurance
    {
        public int count { get; set; }
        public string value { get; set; }
    }

    public class Acceptnewpatient
    {
        public int count { get; set; }
        public bool value { get; set; }
    }

    public class Ismale
    {
        public int count { get; set; }
        public bool value { get; set; }
    }

    public class Language
    {
        public int count { get; set; }
        public string value { get; set; }
    }

    public class SearchNextpageparameters
    {
        public string search { get; set; }
        public string queryTypeodatatype { get; set; }
        public string queryType { get; set; }
        public string select { get; set; }
        public string searchModeodatatype { get; set; }
        public string searchMode { get; set; }
        public string facetsodatatype { get; set; }
        public string[] facets { get; set; }
        public bool count { get; set; }
        public int skip { get; set; }
    }

    public class Value
    {
        public float searchscore { get; set; }
        public string id { get; set; }
        public int searchRank { get; set; }
    }
}