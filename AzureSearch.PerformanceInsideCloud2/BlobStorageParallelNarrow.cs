using System;
using System.Collections.Concurrent;
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
    public static class BlobStorageParallelNarrow
    {
        [FunctionName("BlobStorageParallelNarrow_GetDocuments")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/performance/blobstorage/parallel/narrow/repetitions/{repetitions}")]HttpRequestMessage req,
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
            ConcurrentBag<ProviderNarrow> bag = new ConcurrentBag<ProviderNarrow>();
            List<Task> tasks = new List<Task>();
            for (int r = 0; r < repetitions; r++)
            {
                foreach (string id in ids)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"k-2018-11-20-21-48-47-0857-Utc/{id}.json");
                    tasks.Add(cloudBlockBlob.DownloadTextAsync());
                }
            }

            Task.WaitAll(tasks.ToArray());
            List<ProviderNarrow> providers = new List<ProviderNarrow>();
            foreach (Task<string> task in tasks)
            {
                dynamic p = JsonConvert.DeserializeObject<ProviderNarrow>(task.Result);
                providers.Add(p);
            }
            return req.CreateResponse(
                HttpStatusCode.OK,
                $"{repetitions} repetitions in {nameof(BlobStorageSerial)}->{executionContext.FunctionName}(): {(DateTime.Now - startTime).TotalMilliseconds}, per repetition {(DateTime.Now - startTime).TotalMilliseconds / repetitions}, number of providers returned in total {providers.Count}");
        }
    }
}
