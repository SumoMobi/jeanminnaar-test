using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureSearch.Common;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AzureSearch.PerformanceInsideCloud
{
    public static class BlobStorageParallel
    {
        [FunctionName("BlobStorageParallel_GetDocuments")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/performance/blobstorage/parallel/repetitions/{repetitions}")]HttpRequestMessage req,
            int repetitions,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            List<string> ids = Common.IdsList;
            DateTime startTime = DateTime.Now;
            StorageCredentials storageCredentials = new StorageCredentials(CloudConfigurationManager.GetSetting("storageAccountName"), CloudConfigurationManager.GetSetting("storageAccountKey"));
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("providers");
            ConcurrentBag<KyruusDataStructure> bag = new ConcurrentBag<KyruusDataStructure>();
            List<Task> tasks = new List<Task>();
            for (int r = 0; r < repetitions; r++)
            {
                foreach (string id in ids)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"p-2018-11-12-15-00-01-000726-Utc-4d41468f-51d7-4c4f-9698-24b6637b7eb5/{id}.json");
                    tasks.Add(Task.Run(() =>
                    {
                        string doc = cloudBlockBlob.DownloadText();
                        KyruusDataStructure p = JsonConvert.DeserializeObject<KyruusDataStructure>(doc);
                        bag.Add(p);
                    }));
                }
            }

            Task.WaitAll(tasks.ToArray());
            List<KyruusDataStructure> providers = bag.ToList();    //Accumulate the entries per thread into a single list.
            return req.CreateResponse(
                HttpStatusCode.OK,
                $"{repetitions} repetitions in {nameof(BlobStorageSerial)}->{executionContext.FunctionName}(): {(DateTime.Now - startTime).TotalMilliseconds}, per repetition {(DateTime.Now - startTime).TotalMilliseconds / repetitions}, number of providers returned in total {providers.Count}");
        }
    }
}
