using System;

namespace AzureSearch.Common
{
    public class KyruusDataStructure
    {
        public bool accepting_new_patients { get; set; }
        public object access_program_status { get; set; }
        public Age_Groups_Seen[] age_groups_seen { get; set; }
        public string approach_to_care { get; set; }
        public Board_Certifications[] board_certifications { get; set; }
        public object[] case_types_seen { get; set; }
        public object clinic_location_url { get; set; }
        public object clinical_contact { get; set; }
        public Contact[] contacts { get; set; }
        public string[] credentialed_specialty { get; set; }
        public object credentialing_attachment { get; set; }
        public string current_status { get; set; }
        public string customer_code { get; set; }
        public string date_of_birth { get; set; }
        public Degree[] degrees { get; set; }
        public string external_id { get; set; }
        public string gender { get; set; }
        public object hospital_affiliation_url { get; set; }
        public int id { get; set; }
        public string image_url { get; set; }
        public Insurance_Accepted[] insurance_accepted { get; set; }
        public string interests_activities { get; set; }
        public bool is_live { get; set; }
        public bool is_primary_care { get; set; }
        public bool is_specialty_care { get; set; }
        public Language[] languages { get; set; }
        public string location_change_form { get; set; }
        public Location[] locations { get; set; }
        public string md_anderson { get; set; }
        public Medical_School[] medical_school { get; set; }
        public Metadata metadata { get; set; }
        public object multi_resource_scheduling { get; set; }
        public Name name { get; set; }
        public Network_Affiliations[] network_affiliations { get; set; }
        public Network[] networks { get; set; }
        public string notes { get; set; }
        public string npi { get; set; }
        public string other_profile_edits { get; set; }
        public string p4_id { get; set; }
        public string portal_link { get; set; }
        public Practice_Groups[] practice_groups { get; set; }
        public string preferred_name { get; set; }
        public object professional_statement { get; set; }
        public string provider_email { get; set; }
        public string provider_is_employed { get; set; }
        public string provider_type { get; set; }
        public string research_pubs { get; set; }
        public object reviews { get; set; }
        public Scope_Of_Practice scope_of_practice { get; set; }
        public string show_in_pmac { get; set; }
        public string show_in_pmc { get; set; }
        public Sort_Preferences sort_preferences { get; set; }
        public string specializing_in { get; set; }
        public Specialty[] specialties { get; set; }
        public Status_Transitions[] status_transitions { get; set; }
        public Training[] training { get; set; }
        public string video_url { get; set; }
        public string web_phone_number { get; set; }
        public int? years_in_practice { get; set; }

        public static implicit operator KyruusDataStructure(RelaxedKyruusDataStructure relaxedKyruusDataStructure)
        {   //We get junk entries from Kyruus.  Use this operator to trap these (conversion) exceptions, report and reject the junk entries.
            RelaxedKyruusDataStructure r = relaxedKyruusDataStructure;
            KyruusDataStructure p = new KyruusDataStructure
            {
                accepting_new_patients = r.accepting_new_patients.Value,
                access_program_status = r.access_program_status,
                age_groups_seen = r.age_groups_seen,
                approach_to_care = r.approach_to_care,
                board_certifications = r.board_certifications,
                case_types_seen = r.case_types_seen,
                clinical_contact = r.clinical_contact,
                clinic_location_url = r.clinic_location_url,
                contacts = r.contacts,
                credentialed_specialty = r.credentialed_specialty,
                credentialing_attachment = r.credentialing_attachment,
                current_status = r.current_status,
                customer_code = r.customer_code,
                date_of_birth = r.date_of_birth,
                degrees = r.degrees,
                external_id = r.external_id,
                gender = r.gender,
                hospital_affiliation_url = r.hospital_affiliation_url,
                id = r.id,
                image_url = r.image_url,
                insurance_accepted = r.insurance_accepted,
                interests_activities = r.interests_activities,
                is_live = r.is_live.Value,
                is_primary_care = r.is_primary_care.Value,
                is_specialty_care = r.is_specialty_care.Value,
                languages = r.languages,
                locations = r.locations,
                location_change_form = r.location_change_form,
                md_anderson = r.md_anderson,
                medical_school = r.medical_school,
                metadata = r.metadata,
                multi_resource_scheduling = r.multi_resource_scheduling,
                name = r.name,
                networks = r.networks,
                network_affiliations = r.network_affiliations,
                notes = r.notes,
                npi = r.npi,
                other_profile_edits = r.other_profile_edits,
                p4_id = r.p4_id,
                portal_link = r.portal_link,
                practice_groups = r.practice_groups,
                preferred_name = r.preferred_name,
                professional_statement = r.professional_statement,
                provider_email = r.provider_email,
                provider_is_employed = r.provider_is_employed,
                provider_type = r.provider_type,
                research_pubs = r.research_pubs,
                reviews = r.reviews,
                scope_of_practice = r.scope_of_practice,
                show_in_pmac = r.show_in_pmac,
                show_in_pmc = r.show_in_pmc,
                sort_preferences = r.sort_preferences,
                specializing_in = r.specializing_in,
                specialties = r.specialties,
                status_transitions = r.status_transitions,
                training = r.training,
                video_url = r.video_url,
                web_phone_number = r.web_phone_number,
                years_in_practice = r.years_in_practice
            };
            return p;
        }
    }

    public class RelaxedKyruusDataStructure
    {   //We get providers with almost no field populated.  That crashes JSON Converter.  We use this interim class to weed out the junk entries.
        public bool? accepting_new_patients { get; set; }
        public object access_program_status { get; set; }
        public Age_Groups_Seen[] age_groups_seen { get; set; }
        public string approach_to_care { get; set; }
        public Board_Certifications[] board_certifications { get; set; }
        public object[] case_types_seen { get; set; }
        public object clinic_location_url { get; set; }
        public object clinical_contact { get; set; }
        public Contact[] contacts { get; set; }
        public string[] credentialed_specialty { get; set; }
        public object credentialing_attachment { get; set; }
        public string current_status { get; set; }
        public string customer_code { get; set; }
        public string date_of_birth { get; set; }
        public Degree[] degrees { get; set; }
        public string external_id { get; set; }
        public string gender { get; set; }
        public object hospital_affiliation_url { get; set; }
        public int id { get; set; }
        public string image_url { get; set; }
        public Insurance_Accepted[] insurance_accepted { get; set; }
        public string interests_activities { get; set; }
        public bool? is_live { get; set; }
        public bool? is_primary_care { get; set; }
        public bool? is_specialty_care { get; set; }
        public Language[] languages { get; set; }
        public string location_change_form { get; set; }
        public Location[] locations { get; set; }
        public string md_anderson { get; set; }
        public Medical_School[] medical_school { get; set; }
        public Metadata metadata { get; set; }
        public object multi_resource_scheduling { get; set; }
        public Name name { get; set; }
        public Network_Affiliations[] network_affiliations { get; set; }
        public Network[] networks { get; set; }
        public string notes { get; set; }
        public string npi { get; set; }
        public string other_profile_edits { get; set; }
        public string p4_id { get; set; }
        public string portal_link { get; set; }
        public Practice_Groups[] practice_groups { get; set; }
        public string preferred_name { get; set; }
        public object professional_statement { get; set; }
        public string provider_email { get; set; }
        public string provider_is_employed { get; set; }
        public string provider_type { get; set; }
        public string research_pubs { get; set; }
        public object reviews { get; set; }
        public Scope_Of_Practice scope_of_practice { get; set; }
        public string show_in_pmac { get; set; }
        public string show_in_pmc { get; set; }
        public Sort_Preferences sort_preferences { get; set; }
        public string specializing_in { get; set; }
        public Specialty[] specialties { get; set; }
        public Status_Transitions[] status_transitions { get; set; }
        public Training[] training { get; set; }
        public string video_url { get; set; }
        public string web_phone_number { get; set; }
        public int? years_in_practice { get; set; }
    }

    public class Metadata
    {
        public _Source _source { get; set; }
        public DateTime last_modified { get; set; }
        public DateTime last_updated { get; set; }
    }

    public class _Source
    {
        public object created { get; set; }
        public DateTime last_modified { get; set; }
        public DateTime last_updated { get; set; }
        public object pma_last_updated { get; set; }
    }

    public class Name
    {
        public string first_name { get; set; }
        public string full_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string suffix { get; set; }
    }

    public class Scope_Of_Practice
    {
        public Concept[] concepts { get; set; }
    }

    public class Concept
    {
        public string[] attributes { get; set; }
        public string cui { get; set; }
        public string name { get; set; }
        public string searchability { get; set; }
        public Term[] terms { get; set; }
    }

    public class Term
    {
        public string name { get; set; }
        public string tui { get; set; }
    }

    public class Sort_Preferences
    {
    }

    public class Age_Groups_Seen
    {
        public string name { get; set; }
    }

    public class Board_Certifications
    {
        public string board_name { get; set; }
        public string board_specialty { get; set; }
        public string certification_type { get; set; }
        public string certifying_board { get; set; }
        public int? rank { get; set; }
        public string specialty_name { get; set; }
        public int? year_certified { get; set; }
    }

    public class Contact
    {
        public string contact_type { get; set; }
        public object extension { get; set; }
        public object subtype { get; set; }
        public object value { get; set; }
    }

    public class Degree
    {
        public string name { get; set; }
        public string source { get; set; }
        public object source_url { get; set; }
    }

    public class Insurance_Accepted
    {
        public string name { get; set; }
    }

    public class Language
    {
        public string language { get; set; }
    }

    public class Location
    {
        public string city { get; set; }
        public string commercial_entity_name { get; set; }
        public Contact1[] contacts { get; set; }
        public Coordinates coordinates { get; set; }
        public object distance { get; set; }
        public object email { get; set; }
        public string external_id { get; set; }
        public bool facility_fee { get; set; }
        public string fax { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public object[] networks { get; set; }
        public object office_hours { get; set; }
        public string phone { get; set; }
        public int rank { get; set; }
        public string state { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string suite { get; set; }
        public string type { get; set; }
        public string zip { get; set; }
    }

    public class Coordinates
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class Contact1
    {
        public object ext { get; set; }
        public string subtype { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Medical_School
    {
        public string city { get; set; }
        public string country { get; set; }
        public string graduation_date { get; set; }
        public int? graduation_year { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public object street1 { get; set; }
        public object street2 { get; set; }
        public string zip { get; set; }
    }

    public class Network_Affiliations
    {
        public bool active { get; set; }
        public object billing_tin { get; set; }
        public string created { get; set; }
        public Customer customer { get; set; }
        public int id { get; set; }
        public object logo { get; set; }
        public string modified { get; set; }
        public string name { get; set; }
        public object rank { get; set; }
        public string type { get; set; }
    }

    public class Customer
    {
        public string data_id { get; set; }
        public int default_taxycab_edition { get; set; }
        public int id { get; set; }
        public string index_name { get; set; }
        public string name { get; set; }
    }

    public class Network
    {
        public string network { get; set; }
    }

    public class Practice_Groups
    {
        public bool has_info { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Specialty
    {
        public Alias[] aliases { get; set; }
        public string eui { get; set; }
        public bool is_certified { get; set; }
        public string piped_name { get; set; }
        public object practice_focus { get; set; }
        public string specialty { get; set; }
        public string subspecialty { get; set; }
    }

    public class Alias
    {
        public string aui { get; set; }
        public string name { get; set; }
    }

    public class Status_Transitions
    {
        public string new_status { get; set; }
        public object previous_status { get; set; }
        public DateTime timestamp { get; set; }
        public string user_id { get; set; }
    }

    public class Training
    {
        public object city { get; set; }
        public object country { get; set; }
        public string degree { get; set; }
        public string field_of_study { get; set; }
        public object graduation_date { get; set; }
        public string graduation_year { get; set; }
        public string name { get; set; }
        public int? rank { get; set; }
        public object state { get; set; }
        public object street1 { get; set; }
        public object street2 { get; set; }
        public string type { get; set; }
        public object zip { get; set; }
    }
}
