using AzureSearch.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureSearch.PerformanceInsideCloud2
{
    public static class BlobReader
    {
        [FunctionName("BlobReader")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/BlobReader")] HttpRequestMessage req,
            ILogger log)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                StorageCredentials storageCredentials = new StorageCredentials(
                    Environment.GetEnvironmentVariable("storageAccountName", EnvironmentVariableTarget.Process),
                    Environment.GetEnvironmentVariable("storageAccountKey", EnvironmentVariableTarget.Process));
                CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
                CloudBlobClient client = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference("providers");

                List<string> names = await GetBlobNames(container, "ky-2018-12-12-14-25-00-0078-Utc/");

                int maximumNumberOfThreads = 500;   //TODO make this configurable.
                int loopCount = names.Count / maximumNumberOfThreads;
                if (names.Count % maximumNumberOfThreads > 0)
                {
                    loopCount++;
                }

                List<RelaxedKyruusDataStructure> kyruusDataList = new List<RelaxedKyruusDataStructure>();
                for (int l = 0; l < loopCount; l++)
                {
                    log.LogDebug($"loop {l}", null);
                    ConcurrentBag<RelaxedKyruusDataStructure> kyruusBag = new ConcurrentBag<RelaxedKyruusDataStructure>();
                    List<Task> tasks = new List<Task>();
                    int throughIndex = maximumNumberOfThreads;
                    if (l == loopCount - 1)
                    {   //The last chunk.  Make sure you only create as many threads as there are names left.
                        throughIndex = (names.Count - (l * maximumNumberOfThreads));
                    }
                    for (int t = 0; t < throughIndex; t++)
                    {
                        int i = t + (l * maximumNumberOfThreads);
                        tasks.Add(ReadBlobIntoBag(container, names[i], kyruusBag, log));
                    }
                    Task.WaitAll(tasks.ToArray());
                    kyruusDataList.AddRange(kyruusBag);
                }

                return req.CreateResponse(
                    HttpStatusCode.OK,
                    $"{(DateTime.Now - startTime).TotalMilliseconds} milliseconds, number of blobs {names.Count}, " +
                    $"number of valid blobs {kyruusDataList.Count}, chunk size {maximumNumberOfThreads}, loop count {loopCount}");
            }
            catch(Exception ex)
            {
                log.LogCritical(ex, "Failed to read blobs into memory", null);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        private static async Task<List<string>> GetBlobNames(CloudBlobContainer container, string prefix)
        {
            List<string> names = new List<string>();
            BlobContinuationToken continuationToken = null;
            do
            {
                BlobResultSegment segment = await container.ListBlobsSegmentedAsync(prefix, continuationToken);
                continuationToken = segment.ContinuationToken;
                names.AddRange(segment.Results.Select(s => ((CloudBlockBlob)s).Name));
            }
            while (continuationToken != null);
            return names;
        }
        private static async Task ReadBlobIntoBag(CloudBlobContainer container, string name, ConcurrentBag<RelaxedKyruusDataStructure> kyruusData, ILogger log)
        {
            CloudBlockBlob blob = container.GetBlockBlobReference(name);
            string data = await blob.DownloadTextAsync();
            RelaxedKyruusDataStructure kds;
            try
            {
                kds = JsonConvert.DeserializeObject<RelaxedKyruusDataStructure>(data);
            }
            catch(Exception ex)
            {
                log.LogCritical($"Could not convert {name}", null);
                log.LogDebug(data, null);
                return; //Skip this one.  TODO Count how many...
            }
            if (kds.locations != null && kds.locations.Length > 0 && kds.show_in_pmc != "No")
            {
                kyruusData.Add(kds);
            }
        }
    }
}

