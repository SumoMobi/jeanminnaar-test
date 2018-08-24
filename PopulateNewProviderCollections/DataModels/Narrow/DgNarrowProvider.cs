using System;
using System.Collections.Generic;
using System.Text;

namespace PopulateNewProviderCollections.DataModels.Narrow
{
    /// <summary>
    /// This maps to the DGProviders collection in the bhprovidersdb database in Cosmos DB.
    /// </summary>
    public class DgProvider
    {
        public static implicit operator DgProvider(DataModels.DgProvider dgProvider)
        {
            DgProvider narrow = new DgProvider
            {
                accepting_new_patients = dgProvider.accepting_new_patients,
                age_groups_seen = dgProvider.age_groups_seen,
                approach_to_care = dgProvider.approach_to_care,
                board_certifications = dgProvider.board_certifications,
                clinic_location_url = dgProvider.clinic_location_url,
                credentialed_specialty = dgProvider.credentialed_specialty,
                current_status = dgProvider.current_status,
                date_of_birth = dgProvider.date_of_birth,
                degrees = dgProvider.degrees,
                gender = dgProvider.gender,
                id = dgProvider.id, image_url = dgProvider.image_url,
                insurance_accepted = dgProvider.insurance_accepted,
                interests_activities = dgProvider.interests_activities,
                is_live = dgProvider.is_live,
                is_primary_care = dgProvider.is_primary_care,
                is_specialty_care = dgProvider.is_specialty_care,
                languages = dgProvider.languages,
                last_modified = dgProvider.last_modified,
                last_updated = dgProvider.last_updated,
                locations = dgProvider.locations,
                name = dgProvider.name,
                name_search = dgProvider.name_search,
                networks = dgProvider.networks,
                network_affiliations = dgProvider.network_affiliations,
                preferred_name = dgProvider.preferred_name,
                provider_email = dgProvider.provider_email,
                provider_type = dgProvider.provider_type,
                scope_of_practice = dgProvider.scope_of_practice, search = "",
                scope_of_practice_terms = dgProvider.scope_of_practice_terms,
                search_rank = dgProvider.search_rank,
                specializing_in = dgProvider.specializing_in,
                specialties = dgProvider.specialties,
                specialties_aliases = dgProvider.specialties_aliases,
                training = dgProvider.training,
                years_in_practice = dgProvider.years_in_practice
            };
            return narrow;
        }
        public string id { get; set; }  //Yes
        public string search { get; set; }
        public int search_rank { get; set; }
        public string accepting_new_patients { get; set; }  //Yes
        public string age_groups_seen { get; set; }  //Yes
        public string approach_to_care { get; set; }    //Yes
        public Board_Certifications[] board_certifications { get; set; }    //Yes
        public string clinic_location_url { get; set; } //Yes
        public string credentialed_specialty { get; set; }  //Yes
        public string current_status { get; set; }  //Yes
        public string date_of_birth { get; set; }   //Yes
        public string degrees { get; set; } //Yes
        public string gender { get; set; }  //Yes
        public string image_url { get; set; }   //Yes
        public string insurance_accepted { get; set; }  //Yes
        public string interests_activities { get; set; }    //Yes
        public string is_live { get; set; } //Yes
        public string is_primary_care { get; set; } //Yes
        public string is_specialty_care { get; set; }   //Yes
        public string languages { get; set; }   //Yes
        public Location[] locations { get; set; }   //Yes
        public string last_modified { get; set; }   //Yes
        public string last_updated { get; set; }    //Yes
        public Name name { get; set; }  //Yes
        public string name_search { get; set; }
        public string network_affiliations { get; set; }    //Yes
        public string networks { get; set; }    //Yes
        public string preferred_name { get; set; }  //Yes
        public string provider_email { get; set; }  //Yes
        public string provider_type { get; set; }   //Yes
        public string scope_of_practice { get; set; }   //Yes
        public string[] scope_of_practice_terms { get; set; }
        public string specializing_in { get; set; } //Yes
        public Specialty[] specialties { get; set; }    //Yes
        public string[] specialties_aliases { get; set; }
        public Training[] training { get; set; }    //Yes
        public string years_in_practice { get; set; }   //Yes
        //public string _rid { get; set; }
        //public string _self { get; set; }
        //public string _etag { get; set; }
        //public string _attachments { get; set; }
        //public int _ts { get; set; }
    }

/*    public class Name
    {
        public string first_name { get; set; }
        public string full_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string suffix { get; set; }
    }

    public class Board_Certifications
    {
        public string board_name { get; set; }
        public string certification_type { get; set; }
        public string specialty_name { get; set; }
        public string year_certified { get; set; }
    }

    public class Location
    {
        public string city { get; set; }
        public string commercial_entity_name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public object distance { get; set; }
        public object email { get; set; }
        public string external_id { get; set; }
        public string facility_fee { get; set; }
        public string fax { get; set; }
        public string name { get; set; }
        public object office_hours { get; set; }
        public string phone { get; set; }
        public string rank { get; set; }
        public string state { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string suite { get; set; }
        public string type { get; set; }
        public string zip { get; set; }
    }

    public class Specialty
    {
        public string specialty { get; set; }
        public object[] subspecialty { get; set; }
    }

    public class Training
    {
        public string degree { get; set; }
        public string graduation_year { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string field_of_study { get; set; }
        public string rank { get; set; }
    }
*/
}
