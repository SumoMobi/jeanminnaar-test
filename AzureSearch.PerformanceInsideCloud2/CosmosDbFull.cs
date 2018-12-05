using AzureSearch.Common;
using Microsoft.Azure;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace AzureSearch.PerformanceInsideCloud
{
    public static class CosmosDbFull
    {
        [FunctionName("CosmosDbFull_GetDocuments")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/performance/cosmosdbfull/repetitions/{repetitions}")]HttpRequestMessage req, 
            int repetitions,
            ExecutionContext executionContext,
            ILogger log)
        {
            DateTime startTime = DateTime.Now;
            DocumentClient documentClient = new DocumentClient(new Uri(
                Environment.GetEnvironmentVariable("cosmosUrl", EnvironmentVariableTarget.Process)),
                Environment.GetEnvironmentVariable("cosmosKey", EnvironmentVariableTarget.Process));
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("bhprovidersdb", "Kyruus");
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            List<KyruusDataStructure> providers = new List<KyruusDataStructure>();
            string sql = "SELECT c.id, c.accepting_new_patients, c.age_groups_seen, c.approach_to_care, c.board_certifications, c.clinic_location_url, c.credentialed_specialty, " +
                "c.current_status, c.date_of_birth, c.degrees, c.gender, " + /*c.has_extended_office_hours,*/ "c.image_url, c.insurance_accepted, c.interests_activities, c.is_live, " +
                "c.is_primary_care, c.is_specialty_care, c.languages, c.locations," /*c.last_modified, c.last_updated,*/ + " c.name, c.network_affiliations, c.networks, " + /*c.office_hours,*/
                "c.preferred_name, c.provider_email, c.provider_type, " +/*c.rating_average, c.rating_count,*/ "c.scope_of_practice, c.specializing_in, c.specialties, c.training, " +
                $"c.video_url, c.web_phone_number, c.years_in_practice FROM c WHERE c.id IN ({Common.IdsInClause})";
            for (int r = 0; r < repetitions; r++)
            {
                providers.AddRange(documentClient.CreateDocumentQuery<KyruusDataStructure>(collectionUri, sql, options).ToList());
            }
            return req.CreateResponse(
                HttpStatusCode.OK,
                $"{repetitions} repetitions in {nameof(BlobStorageSerial)}->{executionContext.FunctionName}(): {(DateTime.Now - startTime).TotalMilliseconds}, per repetition {(DateTime.Now - startTime).TotalMilliseconds / repetitions}, number of providers returned in total {providers.Count}");
        }
    }
}
