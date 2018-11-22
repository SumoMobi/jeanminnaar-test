using AzureSearch.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSearch.Performance
{
    public class BlobStorageNarrow
    {
        public static void GetDocuments(string storageAccountKey, string storageAccountName)
        {
            List<string> ids = Common.IdsList;
            DateTime startTime = DateTime.Now;
            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("transformed");
            List<dynamic> providers = new List<dynamic>(ids.Count);
            foreach (string id in ids)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"k-2018-11-20-21-48-47-0857-Utc/{id}.json");
                string doc = cloudBlockBlob.DownloadText();
                dynamic p = JsonConvert.DeserializeObject<KyruusDataStructure>(doc);
                providers.Add(p);
            }
            Console.WriteLine($"{providers.Count} providers from {nameof(BlobStorage)}->{nameof(GetDocuments)}(): {(DateTime.Now - startTime).TotalMilliseconds}");
        }
        public static void GetDocumentsInParallel(string storageAccountKey, string storageAccountName)
        {
            List<string> ids = Common.IdsList;
            DateTime startTime = DateTime.Now;
            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("transformed");
            Task[] tasks = new Task[5];
            tasks[0] = GetDocumentsSection(cloudBlobContainer, ids, 0);
            tasks[1] = GetDocumentsSection(cloudBlobContainer, ids, 1);
            tasks[2] = GetDocumentsSection(cloudBlobContainer, ids, 2);
            tasks[3] = GetDocumentsSection(cloudBlobContainer, ids, 3);
            tasks[4] = GetDocumentsSection(cloudBlobContainer, ids, 4);
            Task.WaitAll(tasks);
            Console.WriteLine($"{25} providers from {nameof(BlobStorage)}->{nameof(GetDocumentsInParallel)}(): {(DateTime.Now - startTime).TotalMilliseconds}");
        }
        private static async Task GetDocumentsSection(CloudBlobContainer cloudBlobContainer, List<string> ids, int section)
        {
            List<dynamic> providers = new List<dynamic>(5);
            int start = section * 5;
            int end = start + 5;
            for (int i = start; i < end; i++)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"k-2018-11-20-21-48-47-0857-Utc/{ids[i]}.json");
                string doc = await cloudBlockBlob.DownloadTextAsync();
                dynamic p = JsonConvert.DeserializeObject<dynamic>(doc);
                providers.Add(p);
            }
            Console.WriteLine($"{providers.Count} providers");
        }
    }
}
