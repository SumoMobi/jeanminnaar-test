using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopulateNewProviderCollections.DataAccess
{
    public class BhProvidersDatabaseDa
    {
        private const string serviceEndPoint = "https://bh-dg-dev-provider-db-cosmos.documents.azure.com:443/";
        private const string authorizationKey = "O1H3t2ngQvqd9VHv8Y3Rz2KI6sShAwNArYqOTLOw3f2Or4ELOpVcdZLnurZO5w3Fj9YIcur4Y57ah9F9W4xPmQ ==";
        public const string DatabaseName = "bhprovidersdb";
        public static Uri DatabaseUri = UriFactory.CreateDatabaseUri(DatabaseName);
        public static DocumentClient DocumentClient = new DocumentClient(new Uri(serviceEndPoint), authorizationKey);

        public enum CollectionNames
        {
            DgProviderConditions,
            DgProviderInsurancesAccepted,
            DgProviderNames,
            DgProviderSpecialties,
            DgProviderPrimarycarePhysicians,
            DgFilteredProviders,
            DgBackupProviders,
            DgNarrowProviders,
            LimtedIndexes
        }

        public static void ViewDatabases()
        {
            IOrderedQueryable<Database> databases = DocumentClient.CreateDatabaseQuery();
            foreach (Database database in databases)
            {
                Console.WriteLine($"id={database.Id}, resourceId={database.ResourceId}");
            }
            //DGProviders
            //            ResourceResponse<Database> response = documentClient.CreateDatabaseIfNotExistsAsync(new Database()..
        }

        public async static Task CreateDatabase(string databaseName)
        {
            //When creating a database, only need to set the ID property.
            ResourceResponse<Database> databaseResponse = await BhProvidersDatabaseDa.DocumentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
            Database database = databaseResponse.Resource;
            Console.WriteLine($"id={database.Id}, resourceId={database.ResourceId}");

            //There is no delete method on the Database type.  You do that from documentClient.
            //await documentClient.DeleteDatabaseAsync(newDatabase.SelfLink?);
            return;
        }

        public async static Task<DocumentCollection> CreateCollection(CollectionNames collectionName)
        {
            //Set throughput and partition key
            int reservedRUs = 9000; //Will have to sleep when we do bulk updates...
            string partitionKey = "/partitionKey";

            PartitionKeyDefinition partitionKeyDefinition = new PartitionKeyDefinition();
            partitionKeyDefinition.Paths.Add(partitionKey); //Weird.  Cannot have more than one partition key...

            RequestOptions requestOptions = new RequestOptions { OfferThroughput = reservedRUs };
            ResourceResponse<DocumentCollection> response = await BhProvidersDatabaseDa.DocumentClient.CreateDocumentCollectionIfNotExistsAsync(
                DatabaseUri,
                new DocumentCollection
                {
                    Id = collectionName.ToString(),
                    PartitionKey = partitionKeyDefinition
                },
                requestOptions
            );
            DocumentCollection collection = response.Resource;
            return collection;
        }

        public static List<DocumentCollection> ViewCollections()
        {
            List<DocumentCollection> collections = BhProvidersDatabaseDa.DocumentClient.CreateDocumentCollectionQuery(DatabaseUri).ToList();
            return collections;
        }
        public static DocumentCollection GetCollection(string collectionName)
        {
            DocumentCollection collection = BhProvidersDatabaseDa.DocumentClient.CreateDocumentCollectionQuery(DatabaseUri)
                .ToList()
                .First(c => c.Id == collectionName);
            return collection;
        }

        public async static Task DeleteCollection(CollectionNames collectioName)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, collectioName.ToString());
            await BhProvidersDatabaseDa.DocumentClient.DeleteDocumentCollectionAsync(collectionUri);

        }
    }
}
