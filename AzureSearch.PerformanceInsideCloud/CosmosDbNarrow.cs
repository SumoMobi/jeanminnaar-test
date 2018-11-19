using AzureSearch.Common;
using Microsoft.Azure;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace AzureSearch.PerformanceInsideCloud
{
    public static class CosmosDbNarrow
    {
        [FunctionName("CosmosDbNarrow_GetDocuments")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/performance/cosmosdbnarrow/repetitions/{repetitions}")]HttpRequestMessage req,
            int repetitions,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            DateTime startTime = DateTime.Now;
            DocumentClient documentClient = new DocumentClient(new Uri(CloudConfigurationManager.GetSetting("cosmosUrl")), CloudConfigurationManager.GetSetting("cosmosKey"));
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("bhprovidersdb", "DGProviders");
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            List<DGProvider> providers = new List<DGProvider>();
            string sql = "SELECT c.id, c.accepting_new_patients, c.age_groups_seen, c.approach_to_care, c.board_certifications, c.clinic_location_url, c.credentialed_specialty, " +
                "c.current_status, c.date_of_birth, c.degrees, c.gender, " + /*c.has_extended_office_hours,*/ "c.image_url, c.insurance_accepted, c.interests_activities, c.is_live, " +
                "c.is_primary_care, c.is_specialty_care, c.languages," /*c.locations, c.last_modified, c.last_updated,*/ + " c.name, c.network_affiliations, c.networks, " + /*c.office_hours,*/
                "c.preferred_name, c.provider_email, c.provider_type, " +/*c.rating_average, c.rating_count,*/ "c.scope_of_practice, c.specializing_in, c.specialties, c.training, " +
                $"c.video_url, c.web_phone_number, c.years_in_practice FROM c WHERE c.id IN ({Common.IdsInClause})";
            for (int r = 0; r < repetitions; r++)
            {
                providers.AddRange(documentClient.CreateDocumentQuery<DGProvider>(collectionUri, sql, options).ToList());
            }
            return req.CreateResponse(
                HttpStatusCode.OK,
                $"{repetitions} repetitions in {nameof(BlobStorageSerial)}->{executionContext.FunctionName}(): {(DateTime.Now - startTime).TotalMilliseconds}, per repetition {(DateTime.Now - startTime).TotalMilliseconds / repetitions}, number of providers returned in total {providers.Count}");
        }
    }
}
