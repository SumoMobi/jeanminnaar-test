using AzureSearch.Common;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AzureSearch.Performance
{
    public class CosmosDb
    {
        public static void GetDocuments(string cosmosUrl, string cosmosKey)
        {
            DateTime startTime = DateTime.Now;
            DocumentClient documentClient = new DocumentClient(new Uri(cosmosUrl), cosmosKey);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("bhprovidersdb", "Kyruus");
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            string sql = $"SELECT * FROM c WHERE c.id IN ({Common.IdsInClause})";
            List<KyruusDataStructure> providers = documentClient.CreateDocumentQuery<KyruusDataStructure>(collectionUri, sql, options).ToList();
            Console.WriteLine($"{providers.Count} providers from CosmosDB->GetDocuments(): {(DateTime.Now - startTime).TotalMilliseconds}");
        }
        public static async Task GetDocumentsStoredProcedure(string cosmosUrl, string cosmosKey)
        {
            DateTime startTime = DateTime.Now;
            DocumentClient documentClient = new DocumentClient(new Uri(cosmosUrl), cosmosKey);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("bhprovidersdb", "Kyruus");

            string spName = "GetByIds";
            string[] par = new string[] { Common.IdsInClause };
            Uri spUri = UriFactory.CreateStoredProcedureUri("bhprovidersdb", "Kyruus", spName);
            StoredProcedureResponse<dynamic> spResponse = await documentClient.ExecuteStoredProcedureAsync<dynamic>(spUri, par);
            if (spResponse.StatusCode != HttpStatusCode.OK)
            {
            }
//            List<Provider> providers = JsonConvert.DeserializeObject<Provider>(spResponse.Response);
            Console.WriteLine($"? providers from {nameof(CosmosDb)}->{nameof(GetDocumentsStoredProcedure)}(): {(DateTime.Now - startTime).TotalMilliseconds}");
            return;
        }
        public static void GetDocumentsSpecificElements(string cosmosUrl, string cosmosKey)
        {
            DateTime startTime = DateTime.Now;
            DocumentClient documentClient = new DocumentClient(new Uri(cosmosUrl), cosmosKey);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("bhprovidersdb", "Kyruus");
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            string sql = "SELECT c.id, c.accepting_new_patients, c.age_groups_seen, c.approach_to_care, c.board_certifications, c.clinic_location_url, c.credentialed_specialty, " +
                "c.current_status, c.date_of_birth, c.degrees, c.gender, " + /*c.has_extended_office_hours,*/ "c.image_url, c.insurance_accepted, c.interests_activities, c.is_live, " +
                "c.is_primary_care, c.is_specialty_care, c.languages, c.locations," /*c.last_modified, c.last_updated,*/ + " c.name, c.network_affiliations, c.networks, " + /*c.office_hours,*/
                "c.preferred_name, c.provider_email, c.provider_type, " +/*c.rating_average, c.rating_count,*/ "c.scope_of_practice, c.specializing_in, c.specialties, c.training, " +
                $"c.video_url, c.web_phone_number, c.years_in_practice FROM c WHERE c.id IN ({Common.IdsInClause})";
            List<KyruusDataStructure> providers = documentClient.CreateDocumentQuery<KyruusDataStructure>(collectionUri, sql, options).ToList();
            Console.WriteLine($"{providers.Count} providers from {nameof(CosmosDb)}->{nameof(GetDocumentsSpecificElements)}(): {(DateTime.Now - startTime).TotalMilliseconds}");
        }
        public static void GetDocumentsSpecificElementsInParallel(string cosmosUrl, string cosmosKey)
        {
            DateTime startTime = DateTime.Now;
            DocumentClient documentClient = new DocumentClient(new Uri(cosmosUrl), cosmosKey);
            Task[] tasks = new Task[5];
            int start = 0;

            for(int i = 0; i < 5; i++)
            {
                string[] idArray = Common.IdsList.Skip(start * i).Take(5).Select(id => "'" + id + "'").ToArray();
                tasks[i] = GetDocumentsFromCosmos(string.Join(",", idArray), documentClient);
            }
            Task.WaitAll(tasks);
//            List<Provider> providers = documentClient.CreateDocumentQuery<Provider>(collectionUri, sql, options).ToList();
            Console.WriteLine($"? providers from {nameof(CosmosDb)}->{nameof(GetDocumentsSpecificElementsInParallel)}(): {(DateTime.Now - startTime).TotalMilliseconds}");
        }
        private static async Task GetDocumentsFromCosmos(string ids, DocumentClient documentClient)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("bhprovidersdb", "Kyruus");
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            string sql = "SELECT c.id, c.accepting_new_patients, c.age_groups_seen, c.approach_to_care, c.board_certifications, c.clinic_location_url, c.credentialed_specialty, " +
                "c.current_status, c.date_of_birth, c.degrees, c.gender, " + /*c.has_extended_office_hours,*/ "c.image_url, c.insurance_accepted, c.interests_activities, c.is_live, " +
                "c.is_primary_care, c.is_specialty_care, c.languages, c.locations," /*c.last_modified, c.last_updated,*/ + " c.name, c.network_affiliations, c.networks, " + /*c.office_hours,*/
                "c.preferred_name, c.provider_email, c.provider_type, " +/*c.rating_average, c.rating_count,*/ "c.scope_of_practice, c.specializing_in, c.specialties, c.training, " +
                $"c.video_url, c.web_phone_number, c.years_in_practice FROM c WHERE c.id IN ({ids})";
            List<KyruusDataStructure> providers = documentClient.CreateDocumentQuery<KyruusDataStructure>(collectionUri, sql, options).ToList();
            Console.WriteLine($"{providers.Count} providers");
        }
    }
}

