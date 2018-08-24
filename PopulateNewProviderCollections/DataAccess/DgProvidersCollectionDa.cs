using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using PopulateNewProviderCollections.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopulateNewProviderCollections.DataAccess
{
    public class DgProvidersCollectionDa
    {
        private const string collectionName = "DgBackupProviders";
        private static Uri collectionUri => UriFactory.CreateDocumentCollectionUri(BhProvidersDatabaseDa.DatabaseName, collectionName);
        /// <summary>
        /// Called after the main collection (DGProviders) has been updated.
        /// </summary>
        /// <returns>The physicians that do want to be shown on the Banner site and those with locations.</returns>
        public static List<DgProvider> GetFiltered()
        {
            //            DocumentCollection collection = BhProvidersDatabaseDa.GetCollection("DGProviders");

            string sql = "SELECT * FROM c WHERE ARRAY_LENGTH(c.locations) > 0 AND LOWER(c.show_in_pmc) = 'yes'";
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

            List<DgProvider> providers = BhProvidersDatabaseDa.DocumentClient.CreateDocumentQuery<DgProvider>(collectionUri, sql, options).ToList();
            return providers;
        }

        public static List<DgProvider> GetAll()
        {
            string sql = "SELECT * FROM c";
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

            List<DgProvider> providers = BhProvidersDatabaseDa.DocumentClient.CreateDocumentQuery<DgProvider>(collectionUri, sql, options).ToList();
            return providers;
        }

        public static void CreateBackup(List<DgProvider> providers)
        {
            Uri uri = UriFactory.CreateDocumentCollectionUri(BhProvidersDatabaseDa.DatabaseName, BhProvidersDatabaseDa.CollectionNames.DgBackupProviders.ToString());
            foreach (DgProvider provider in providers)
            {
                BhProvidersDatabaseDa.DocumentClient.CreateDocumentAsync(uri, provider);
            }
            Console.WriteLine($"Backup contains {providers.Count} entities");
        }

        public static void PopulateFilteredCollection(List<DgProvider> providers)
        {
            int skippedCount = 0;
            Uri uri = UriFactory.CreateDocumentCollectionUri(BhProvidersDatabaseDa.DatabaseName, BhProvidersDatabaseDa.CollectionNames.DgFilteredProviders.ToString());
            foreach (DgProvider provider in providers)
            {
                if (provider.locations == null || provider.locations.Length == 0 || string.Compare(provider.show_in_pmc, "Yes", true) != 0)
                {
                    skippedCount++;
                    continue;
                }
                BhProvidersDatabaseDa.DocumentClient.CreateDocumentAsync(uri, provider);
            }
            Console.WriteLine($"Received {providers.Count} and skipped {skippedCount}");
        }

        public async static Task PopulateNarrowCollection(List<DgProvider> filteredProviders)
        {
            DocumentCollection narrowProviderCollection = new DocumentCollection { Id = BhProvidersDatabaseDa.CollectionNames.DgNarrowProviders.ToString() };
            narrowProviderCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;

/*            Index index = new Index();
            narrowProviderCollection.IndexingPolicy.IncludedPaths = new System.Collections.ObjectModel.Collection<IncludedPath>()
            {
                new IncludedPath
                {
                    Indexes = new System.Collections.ObjectModel.Collection<Index>()
                    { new  ;
*/
            await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgNarrowProviders);

            Uri uri = UriFactory.CreateDocumentCollectionUri(BhProvidersDatabaseDa.DatabaseName, BhProvidersDatabaseDa.CollectionNames.DgNarrowProviders.ToString());
            foreach (DgProvider provider in filteredProviders)
            {
                DataModels.Narrow.DgProvider narrow = provider;
                Task<ResourceResponse<Document>> task = BhProvidersDatabaseDa.DocumentClient.CreateDocumentAsync(uri, narrow);
            }
            Console.WriteLine($"Received {filteredProviders.Count}");
        }
        public async static Task PopulateLimitedIndexesCollection(List<DgProvider> narrowProviders)
        {
            Uri uri = UriFactory.CreateDocumentCollectionUri(BhProvidersDatabaseDa.DatabaseName, BhProvidersDatabaseDa.CollectionNames.LimtedIndexes.ToString());
            //Delete collection if exists
            bool exists = BhProvidersDatabaseDa.DocumentClient.CreateDocumentCollectionQuery(BhProvidersDatabaseDa.DatabaseUri)
                .ToList()
                .Exists(c => c.Id == BhProvidersDatabaseDa.CollectionNames.LimtedIndexes.ToString());
            if (exists)
            {
                await BhProvidersDatabaseDa.DocumentClient.DeleteDocumentCollectionAsync(uri);
            }

            //Set throughput and partition key
            int reservedRUs = 9000;
            string partitionKey = "/locations[0].zip";

            PartitionKeyDefinition partitionKeyDefinition = new PartitionKeyDefinition();
            partitionKeyDefinition.Paths.Add(partitionKey); //Weird.  Cannot have more than one partition key...

            RequestOptions requestOptions = new RequestOptions { OfferThroughput = reservedRUs };
            DocumentCollection limitedIndexesCollection = new DocumentCollection { Id = BhProvidersDatabaseDa.CollectionNames.LimtedIndexes.ToString() };
            limitedIndexesCollection.IndexingPolicy.IncludedPaths = new System.Collections.ObjectModel.Collection<IncludedPath>();
            limitedIndexesCollection.IndexingPolicy.IncludedPaths.Add
                (new IncludedPath
                {
                    Indexes = new System.Collections.ObjectModel.Collection<Index>
                    {
                        new HashIndex(DataType.String)
                        {
                            Precision = -1
                        },
                        new RangeIndex(DataType.String)
                        {
                            Precision = -1
                        }
                    },
                    Path = "/whateverPath/*"
                });
            limitedIndexesCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            ResourceResponse<DocumentCollection> response = await BhProvidersDatabaseDa.DocumentClient.CreateDocumentCollectionAsync(
                BhProvidersDatabaseDa.DatabaseUri,
                new DocumentCollection
                {
                    Id = BhProvidersDatabaseDa.CollectionNames.LimtedIndexes.ToString(),
                    PartitionKey = partitionKeyDefinition
                },
                requestOptions
            );
            DocumentCollection collection = response.Resource;
            DocumentCollection narrowProviderCollection = new DocumentCollection { Id = BhProvidersDatabaseDa.CollectionNames.DgNarrowProviders.ToString() };
            narrowProviderCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;

            /*            Index index = new Index();
                        narrowProviderCollection.IndexingPolicy.IncludedPaths = new System.Collections.ObjectModel.Collection<IncludedPath>()
                        {
                            new IncludedPath
                            {
                                Indexes = new System.Collections.ObjectModel.Collection<Index>()
                                { new  ;
            */
            await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgNarrowProviders);

            foreach (DgProvider provider in filteredProviders)
            {
                DataModels.Narrow.DgProvider narrow = provider;
                Task<ResourceResponse<Document>> task = BhProvidersDatabaseDa.DocumentClient.CreateDocumentAsync(uri, narrow);
            }
            Console.WriteLine($"Received {filteredProviders.Count}");
        }
    }
}
