using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureSearch.Loader
{
    public class IndexLoader
    {
        public static void MergeOrUpload<T>(List<T> indexDataList, string apiKey, string serviceName, string indexName) where T : class
        {
            SearchServiceClient serviceClient = new SearchServiceClient(serviceName, new SearchCredentials(apiKey));
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient(indexName);
            Console.WriteLine($"API Version {indexClient.ApiVersion}");
            int chunkSize = 500;
            int chunks = indexDataList.Count / chunkSize;
            if (indexDataList.Count % chunkSize > 0)
            {
                chunks++;
            }
            for (int l = 0; l < chunks; l++)
            {
                int max = chunkSize;
                if (l == chunks)
                {
                    max = indexDataList.Count - (chunkSize * (l - 1));
                }
                IndexBatch<T> batch = IndexBatch.MergeOrUpload<T>(indexDataList.Skip(l * chunkSize).Take(max));
                try
                {
                    DocumentIndexResult result = indexClient.Documents.Index(batch);
                }
                catch (IndexBatchException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
    }
}
