
public class ProviderNarrow
{
    public string id { get; set; }
    public string search { get; set; }
    public int search_rank { get; set; }
    public string accepting_new_patients { get; set; }
    public string age_groups_seen { get; set; }
    public string approach_to_care { get; set; }
    public Board_Certifications[] board_certifications { get; set; }
    public string clinic_location_url { get; set; }
    public string credentialed_specialty { get; set; }
    public string current_status { get; set; }
    public string date_of_birth { get; set; }
    public string degrees { get; set; }
    public string gender { get; set; }
    public string image_url { get; set; }
    public string insurance_accepted { get; set; }
    public string interests_activities { get; set; }
    public string is_live { get; set; }
    public string is_primary_care { get; set; }
    public string is_specialty_care { get; set; }
    public string languages { get; set; }
    public Location[] locations { get; set; }
    public string last_modified { get; set; }
    public string last_updated { get; set; }
    public Name name { get; set; }
    public string name_search { get; set; }
    public string network_affiliations { get; set; }
    public string networks { get; set; }
    public string preferred_name { get; set; }
    public string provider_email { get; set; }
    public string provider_type { get; set; }
    public string research_pubs { get; set; }
    public object rating_average { get; set; }
    public object rating_count { get; set; }
    public string scope_of_practice { get; set; }
    public string[] scope_of_practice_terms { get; set; }
    public string show_in_pmac { get; set; }
    public string show_in_pmc { get; set; }
    public string specializing_in { get; set; }
    public Specialty[] specialties { get; set; }
    public string[] specialties_aliases { get; set; }
    public Training[] training { get; set; }
    public string video_url { get; set; }
    public object web_phone_number { get; set; }
    public string years_in_practice { get; set; }
}

public class Name
{
    public string first_name { get; set; }
    public string full_name { get; set; }
    public string last_name { get; set; }
    public string middle_name { get; set; }
    public object suffix { get; set; }
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
    public object has_extended_office_hours { get; set; }
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
    public string[] subspecialty { get; set; }
}

public class Training
{
    public string degree { get; set; }
    public string field_of_study { get; set; }
    public string graduation_year { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string rank { get; set; }
}
