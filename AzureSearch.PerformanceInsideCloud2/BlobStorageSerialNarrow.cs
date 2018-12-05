using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureSearch.Common;
using AzureSearch.Common.Dg;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureSearch.PerformanceInsideCloud
{
    public static class BlobStorageSerialNarrow
    {
        /// <summary>
        /// Gets the documents serially (one after the next).
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("BlobStorageSerialNarrow_GetDocuments")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/performance/blobstorage/serially/narrow/repetitions/{repetitions}")]HttpRequestMessage req,
            int repetitions,
            ExecutionContext executionContext,
            ILogger log)
        {
            List<string> ids = Common.IdsList;
            DateTime startTime = DateTime.Now;
            StorageCredentials storageCredentials = new StorageCredentials(
                Environment.GetEnvironmentVariable("storageAccountName", EnvironmentVariableTarget.Process),
                Environment.GetEnvironmentVariable("storageAccountKey", EnvironmentVariableTarget.Process));
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("transformed");
            List<ProviderNarrow> providers = new List<ProviderNarrow>(ids.Count);
            for (int r = 0; r < repetitions; r++)
            {
                foreach (string id in ids)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"k-2018-11-20-21-48-47-0857-Utc/{id}.json");
                    string doc = await cloudBlockBlob.DownloadTextAsync();
                    ProviderNarrow p = JsonConvert.DeserializeObject<ProviderNarrow>(doc);
                    providers.Add(p);
                }
            }

            return req.CreateResponse(
                HttpStatusCode.OK,
                $"{repetitions} repetitions in {nameof(BlobStorageSerial)}->{executionContext.FunctionName}(): {(DateTime.Now - startTime).TotalMilliseconds}, per repetition {(DateTime.Now - startTime).TotalMilliseconds / repetitions}");
        }
    }
}
