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
    public class BlobStorage
    {
        public static void GetDocuments(string storageAccountKey, string storageAccountName)
        {
            List<string> ids = Common.IdsList;
            DateTime startTime = DateTime.Now;
            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
            CloudStorageAccount cloudStorageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("providers");
            List<KyruusDataStructure> providers = new List<KyruusDataStructure>(ids.Count);
            foreach (string id in ids)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"p-2018-11-12-15-00-01-000726-Utc-4d41468f-51d7-4c4f-9698-24b6637b7eb5/{id}.json");
                string doc = cloudBlockBlob.DownloadText();
                KyruusDataStructure p = JsonConvert.DeserializeObject<KyruusDataStructure>(doc);
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
            CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("providers");
            Task[] tasks = new Task[5];
            tasks[0] = GetDocumentsSection(cloudBlobContainer, ids, 0);
            tasks[1] = GetDocumentsSection(cloudBlobContainer, ids, 1);
            tasks[2] = GetDocumentsSection(cloudBlobContainer, ids, 2);
            tasks[3] = GetDocumentsSection(cloudBlobContainer, ids, 3);
            tasks[4] = GetDocumentsSection(cloudBlobContainer, ids, 4);
            Task.WaitAll(tasks);
            Console.WriteLine($"{25} providers from {nameof(BlobStorage)}->{nameof(GetDocumentsInParallel)}(): {(DateTime.Now - startTime).TotalMilliseconds}");
        }
        public static async Task GetDocumentsSection(CloudBlobContainer cloudBlobContainer, List<string> ids, int section)
        {
            List<KyruusDataStructure> providers = new List<KyruusDataStructure>(10);
            int start = section * 5;
            int end = start + 5;
            for (int i = start; i < end; i++)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference($"p-2018-11-12-15-00-01-000726-Utc-4d41468f-51d7-4c4f-9698-24b6637b7eb5/{ids[i]}.json");
                string doc = await cloudBlockBlob.DownloadTextAsync();
                KyruusDataStructure p = JsonConvert.DeserializeObject<KyruusDataStructure>(doc);
                providers.Add(p);
            }
            Console.WriteLine($"{providers.Count} providers");
        }
    }
}
