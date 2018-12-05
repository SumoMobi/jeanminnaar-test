using System.Collections.Generic;

namespace AzureSearch.Api.ResponseStructures.Prior
{
    public class Filters
    {
        public Location Location { get; set; }
        public List<Insuranceaccepted> InsuranceAccepted { get; set; }
        public string[] AcceptingNewPatients { get; set; }
        public List<AgeGroupsSeen> AgeGroupsSeen { get; set; }
        public List<ProviderGender> ProviderGender { get; set; }
        public List<Providertype> ProviderType { get; set; }
        public List<Language> Language { get; set; }
        public List<Hospitalaffiliations> HospitalAffiliations { get; set; }
        public int Count { get; set; }
    }

    public class Location
    {
    }

    public class Insuranceaccepted
    {
        public string Name { get; set; }
        public string Ids { get; set; }
    }

    public class AgeGroupsSeen
    {
        public string Name { get; set; }
        public string Ids { get; set; }
    }

    public class ProviderGender
    {
        public string Name { get; set; }
        public string Ids { get; set; }
    }

    public class Providertype
    {
        public string Name { get; set; }
        public string Ids { get; set; }
    }

    public class Language
    {
        public string Name { get; set; }
        public string Ids { get; set; }
    }

    public class Hospitalaffiliations
    {
        public string Name { get; set; }
        public string Ids { get; set; }
    }
}