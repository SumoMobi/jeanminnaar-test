using Microsoft.Azure.Search;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace AzureSearch.Api
{
    public static class AzureSearchConnectionCache
    {
        public enum IndexNames { conditions, insurances, locations, names, providers, specilties }

        private static MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public static ISearchIndexClient GetIndexClient(IndexNames indexName)
        {
            ISearchIndexClient indexClient;
            if (_memoryCache.TryGetValue(indexName.ToString(), out indexClient))
            {
                return indexClient;
            }
            indexClient = SearchServiceClient.Indexes.GetClient(indexName.ToString());
            _memoryCache.Set(indexName.ToString(), indexClient);
            return indexClient;
        }
        private static SearchServiceClient SearchServiceClient
        {
            get
            {
                SearchServiceClient serviceClient;
                if (_memoryCache.TryGetValue("azSearchService", out serviceClient))
                {
                    return serviceClient;
                }
                serviceClient = new SearchServiceClient(
                    Environment.GetEnvironmentVariable("serviceName", EnvironmentVariableTarget.Process),
                    new SearchCredentials(Environment.GetEnvironmentVariable("apiKey", EnvironmentVariableTarget.Process)
                ));
                _memoryCache.Set("azSearchService", serviceClient);
                return serviceClient;
            }
        }

    }
}
