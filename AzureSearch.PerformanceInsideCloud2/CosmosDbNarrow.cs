using AzureSearch.Common;
using AzureSearch.Common.Dg;
using Microsoft.Azure;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureSearch.PerformanceInsideCloud
{
    public static class CosmosDbNarrow
    {

        [FunctionName("CosmosDbNarrow_GetDocuments")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/performance/cosmosdbnarrow/repetitions/{repetitions}")]HttpRequestMessage req,
            int repetitions,
            ExecutionContext executionContext,
            ILogger log)
        {
            DocumentClient documentClient = new DocumentClient(new Uri(
                Environment.GetEnvironmentVariable("cosmosUrl", EnvironmentVariableTarget.Process)),
                Environment.GetEnvironmentVariable("cosmosKey", EnvironmentVariableTarget.Process));
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("bhprovidersdb", "DGProviders");
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

            DateTime startTime = DateTime.Now;
            List<ProviderNarrow> providers = new List<ProviderNarrow>();
            string sql = "SELECT c.id, c.accepting_new_patients, c.age_groups_seen, c.approach_to_care, c.board_certifications, c.clinic_location_url, c.credentialed_specialty, " +
                "c.current_status, c.date_of_birth, c.degrees, c.gender, " + /*c.has_extended_office_hours,*/ "c.image_url, c.insurance_accepted, c.interests_activities, c.is_live, " +
                "c.is_primary_care, c.is_specialty_care, c.languages,c.locations, c.last_modified, c.last_updated, c.name, c.network_affiliations, c.networks, " + /*c.office_hours,*/
                "c.preferred_name, c.provider_email, c.provider_type, " +/*c.rating_average, c.rating_count,*/ "c.scope_of_practice, c.specializing_in, c.specialties, c.training, " +
                "c.video_url, c.web_phone_number, c.years_in_practice FROM c WHERE c.id = ";
            List<Task> tasks = new List<Task>();
            for (int r = 0; r < repetitions; r++)
            {
                for (int i = 0; i < Common.IdsList.Count; i++)
                {
                    string sql2 = $"{sql}'{Common.IdsList[i]}'";
                    tasks.Add(Task.Run(() => documentClient.CreateDocumentQuery<ProviderNarrow>(collectionUri, sql2, options)));
                }
            }
            Task.WaitAll(tasks.ToArray());
            return req.CreateResponse(
                HttpStatusCode.OK,
                $"{repetitions} repetitions in {nameof(CosmosDbNarrow)}->{executionContext.FunctionName}(): {(DateTime.Now - startTime).TotalMilliseconds}, per repetition {(DateTime.Now - startTime).TotalMilliseconds / repetitions}, number of providers returned in total {repetitions * Common.IdsList.Count}.");
        }
    }
}
