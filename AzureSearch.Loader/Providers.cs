using AzureSearch.Common;
using AzureSearch.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearch.Loader
{
    public class ProviderIndex
    {
        public string id { get; set; }
        public int searchRank { get; set; }
        public int randomNumber { get; set; }
        public List<string> agesSeen { get; set; }
        public List<string> acceptedInsurances { get; set; }
        public List<string> specialties { get; set; }
        public List<string> conditions { get; set; }
        public string firstAndLastName { get; set; }
        public string firstAndLastNameLower { get; set; }
        public bool acceptNewPatients { get; set; }
        public bool isMale { get; set; }
        public string providerType { get; set; }
        public List<string> languages { get; set; }
        public List<string> networkAffiliations { get; set; }
        public List<string> locationIds { get; set; }
        public List<string> acceptedInsurancesLower { get; set; }
        public List<string> conditionsLower { get; set; }
        public List<string> specialtiesLower { get; set; }
        public List<string> cities { get; set; }
        public List<string> zipCodes { get; set; }

    }
    public class Providers
    {
        public static async Task Upload(string apiKey, string serviceName)
        {
            //Get all the provider data.
            DateTime startDateTime = DateTime.Now;
            List<KyruusDataStructure> providers = ProviderDa.GetAllFromDownload();
            Console.WriteLine($"{providers.Count} providers fetched.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            startDateTime = DateTime.Now;
            List<string> highRankingNetworks = new List<string>() { "bann", "bep", "bhwr", "bumg" };    //TODO Need to get this from app settings.

            Random random = new Random(DateTime.Now.Millisecond);

            List<ProviderIndex> providerIndexList = new List<ProviderIndex>();
            foreach (KyruusDataStructure p in providers)
            {
                #region Assuming properties can be null.  This is what the code here is about -- expecting the worst.
                //Accepted insurances
                List<string> acceptedInsurances = new List<string>();
                if (p.insurance_accepted != null)
                {
                    acceptedInsurances = p.insurance_accepted
                        .Where(i => string.IsNullOrWhiteSpace(i.name) == false)
                        .Select(i => i.name)
                        .ToList();
                }
                //Ages seen
                List<string> agesSeen = new List<string>();
                if (p.age_groups_seen != null)
                {
                    agesSeen = p.age_groups_seen
                        .Where(a => string.IsNullOrWhiteSpace(a.name) == false)
                        .Select(a => a.name)
                        .ToList();
                }
                //Cities
                List<string> cities = new List<string>();
                if (p.locations != null)
                {
                    cities = p.locations
                        .Where(l => string.IsNullOrWhiteSpace(l.city) == false)
                        .Select(l => l.city)
                        .ToList();
                }
                //Conditions
                List<string> conditions = new List<string>();
                if (p.scope_of_practice != null && p.scope_of_practice.concepts != null)
                {
                    conditions = p.scope_of_practice.concepts
                        .Where(c => c.searchability == "searchable" && string.IsNullOrWhiteSpace(c.name) == false)
                        .Select(c => c.name)
                        .ToList();
                }
                //Languages
                List<string> languages = new List<string>();
                if (p.languages != null)
                {
                    languages = p.languages
                        .Where(l => string.IsNullOrWhiteSpace(l.language) == false)
                        .Select(l => l.language)
                        .ToList();
                }
                //Location IDs
                List<string> locationIds = new List<string>();
                if (p.locations != null)
                {
                    locationIds = p.locations
                        .Where(l => string.IsNullOrWhiteSpace(l.id) == false)
                        .Select(l => l.id)
                        .ToList();
                }
                //Names
                //p.nane.full_name does not have the p.preferred_name in the first name part.
                string firstName = string.Empty;
                string lastName = string.Empty;
                string firstAndLastName = string.Empty;
                if (p.name != null)
                {
                    if (string.IsNullOrWhiteSpace(p.preferred_name))
                    {
                        firstName = p.name.first_name;
                    }
                    else
                    {
                        firstName = p.preferred_name;
                    }
                    if (string.IsNullOrWhiteSpace(firstName))
                    {
                        firstName = string.Empty;
                    }
                    lastName = p.name.last_name;
                    if (string.IsNullOrWhiteSpace(lastName))
                    {
                        lastName = string.Empty;
                    }
                }
                if (firstName.Length == 0)
                {
                    firstAndLastName = lastName;
                }
                else
                {
                    firstAndLastName = firstName + " " + lastName;
                }
                //Network affiliations
                List<string> networkAffiliations = new List<string>();
                if (p.network_affiliations != null)
                {
                    networkAffiliations = p.network_affiliations
                        .Where(n => n.type == "Hospital" && string.IsNullOrWhiteSpace(n.name) == false)
                        .Select(n => n.name)
                        .ToList();
                }
                //Specialties
                List<string> specialties = new List<string>();
                if (p.specialties != null)
                {
                    //Add specialties
                    specialties = p.specialties
                        .Select(s => s.specialty)
                        .ToList();
                    //Add subspecialties
                    specialties.AddRange(
                        p.specialties
                        .Select(s => s.subspecialty)
                        .ToList());
                    //Add aliases
                    specialties.AddRange(
                        p.specialties
                        .SelectMany(s => s.aliases)
                        .Select(a => a.name)
                        .ToList());
                    //Remove any null entries
                    specialties = specialties
                        .Where(s => string.IsNullOrWhiteSpace(s) == false)
                        .ToList();
                    //Distinct the list
                    specialties = specialties
                        .Distinct()
                        .ToList();
                }
                int searchRank = 100;
                if (p.networks != null)
                {
                    if (highRankingNetworks
                        .Intersect(p.networks.Select(n => n.network.ToLower())
                        .ToList())
                        .Count() > 0)
                    {
                        searchRank = 0;
                    }
                }
                //Zip codes
                List<string> zipCodes = new List<string>();
                if (p.locations != null)
                {
                    zipCodes = p.locations
                        .Where(l => string.IsNullOrWhiteSpace(l.zip) == false)
                        .Select(l => l.zip)
                        .ToList();
                }
                #endregion

                ProviderIndex pi = new ProviderIndex
                {
                    acceptedInsurances = acceptedInsurances,
                    acceptedInsurancesLower = acceptedInsurances
                        .Select(i => i.ToLower())
                        .ToList(),
                    acceptNewPatients = p.accepting_new_patients,
                    agesSeen = agesSeen,
                    cities = cities,
                    conditions = conditions,
                    conditionsLower = conditions.Select(c => c.ToLower()).ToList(), //TODO recheck when you need lower.
                    firstAndLastName = firstAndLastName,
                    firstAndLastNameLower = firstAndLastName.ToLower(),
                    id = p.id.ToString(),
                    isMale = p.gender == "Male",
                    languages = languages,
                    locationIds = locationIds,
                    networkAffiliations = networkAffiliations,
                    providerType = p.provider_type,
                    randomNumber = random.Next(0, int.MaxValue),    //Use a huge random range to minimize chances of getting the same number again later on.
                    searchRank = searchRank,
                    specialties = specialties,
                    specialtiesLower = specialties.Select(s => s.ToLower()).ToList(),
                    zipCodes = zipCodes
                };
                providerIndexList.Add(pi);
            }
            Console.WriteLine($"{providers.Count} providers in memory.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Upload the data to Azure Search
            startDateTime = DateTime.Now;
            IndexLoader.MergeOrUpload(providerIndexList, apiKey, serviceName, "providers", 500);
            Console.WriteLine($"Index uploaded.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

        }

    }
}
