using Newtonsoft.Json;
using System.Collections.Generic;

namespace AzureSearch.Common
{
    public class DGProvider
    {
        [JsonProperty()] public string id { get; set; }
        [JsonProperty()] public string search { get; set; }
        [JsonProperty()] public int search_rank { get; set; }
        [JsonProperty()] public string accepting_new_patients { get; set; }
        [JsonProperty()] public string age_groups_seen { get; set; }
        [JsonProperty()] public string approach_to_care { get; set; }
        [JsonProperty()] public IList<DGBoardCertification> board_certifications { get; set; }
        [JsonProperty()] public string clinic_location_url { get; set; }
        [JsonProperty()] public string credentialed_specialty { get; set; }
        [JsonProperty()] public string current_status { get; set; }
        [JsonProperty()] public string date_of_birth { get; set; }
        [JsonProperty()] public string degrees { get; set; }
        [JsonProperty()] public string gender { get; set; }
        [JsonProperty()] public string image_url { get; set; }
        [JsonProperty()] public string insurance_accepted { get; set; }
        [JsonProperty()] public string interests_activities { get; set; }
        [JsonProperty()] public string is_live { get; set; }
        [JsonProperty()] public string is_primary_care { get; set; }
        [JsonProperty()] public string is_specialty_care { get; set; }
        [JsonProperty()] public string languages { get; set; }
        [JsonProperty()] public IList<DGLocation> locations { get; set; }
        [JsonProperty()] public string last_modified { get; set; }
        [JsonProperty()] public string last_updated { get; set; }
        [JsonProperty()] public DGName name { get; set; }
        [JsonProperty()] public string name_search { get; set; }
        [JsonProperty()] public string network_affiliations { get; set; }
        [JsonProperty()] public string networks { get; set; }
        [JsonProperty()] public string preferred_name { get; set; }
        [JsonProperty()] public string provider_email { get; set; }
        [JsonProperty()] public string provider_type { get; set; }
        [JsonProperty()] public string scope_of_practice { get; set; }
        [JsonProperty()] public IList<string> scope_of_practice_terms { get; set; }
        [JsonProperty()] public string show_in_pmac { get; set; }
        [JsonProperty()] public string show_in_pmc { get; set; }
        [JsonProperty()] public string specializing_in { get; set; }
        [JsonProperty()] public IList<DGSpecialty> specialties { get; set; }
        [JsonProperty()] public IList<string> specialties_aliases { get; set; }
        [JsonProperty()] public IList<DGTraining> training { get; set; }
        [JsonProperty()] public string video_url { get; set; }
        [JsonProperty()] public string web_phone_number { get; set; }
        [JsonProperty()] public string years_in_practice { get; set; }
        public List<string> TypeAhead_Name { get; set; }
        public List<string> TypeAhead_Specialties { get; set; }
        public List<string> TypeAhead_Specialties_PrimaryCare { get; set; }
        public List<string> TypeAhead_ScopeOfPractices { get; set; }
        public List<string> TypeAhead_Insurances { get; set; }
        public string insurance_accepted_lowercase { get; set; }



    }

    public class DGSpecialty
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string specialty { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public IList<string> subspecialty { get; set; }
    }

    public class DGTraining
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string city { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string country { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string degree { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string field_of_study { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string graduation_date { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string graduation_year { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string rank { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string state { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string street1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string street2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string zip { get; set; }

    }

    public class DGName
    {
        [JsonProperty()] public string first_name { get; set; }
        [JsonProperty()] public string full_name { get; set; }
        [JsonProperty()] public string last_name { get; set; }
        [JsonProperty()] public string middle_name { get; set; }
        [JsonProperty()] public string suffix { get; set; }
    }

    public class DGLocation
    {
        [JsonProperty()] public string city { get; set; }
        [JsonProperty()] public string commercial_entity_name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public List<DGContact> contacts { get; set; }
        [JsonProperty()] public string Latitude { get; set; }
        [JsonProperty()] public string Longitude { get; set; }
        [JsonProperty()] public string distance { get; set; }
        [JsonProperty()] public string email { get; set; }
        [JsonProperty()] public string external_id { get; set; }
        [JsonProperty()] public string facility_fee { get; set; }
        [JsonProperty()] public string fax { get; set; }
        [JsonProperty()] public string name { get; set; }
        [JsonProperty()] public string office_hours { get; set; }
        [JsonProperty()] public string phone { get; set; }
        [JsonProperty()] public string rank { get; set; }
        [JsonProperty()] public string state { get; set; }
        [JsonProperty()] public string street1 { get; set; }
        [JsonProperty()] public string street2 { get; set; }
        [JsonProperty()] public object suite { get; set; }
        [JsonProperty()] public string type { get; set; }
        [JsonProperty()] public string zip { get; set; }
    }

    public class DGContact
    {
        [JsonProperty()] public string contact_type { get; set; }
        [JsonProperty()] public string extension { get; set; }
        [JsonProperty()] public string subtype { get; set; }
        [JsonProperty()] public string value { get; set; }
    }

    public class DGBoardCertification
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string board_name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string certification_type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string specialty_name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public string year_certified { get; set; }
    }
}
