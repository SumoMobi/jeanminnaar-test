using AzureSearch.Common;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureSearch.Upload
{
    public class ProviderDa
    {
        private const string serviceEndPoint = "https://bh-dg-dev4-provider-db-cosmos.documents.azure.com:443/";
        private const string authorizationKey = "1RTbg246pfmbLeSlmFZFnGyUeVauuolDXZmojTK5Hgdy015UjyQdb5k90tV7XvEBmaP77QTODLCy3EsGzVjFUQ==";
        const string _databaseName = "bhprovidersdb";
        static Uri _databaseUri = UriFactory.CreateDatabaseUri(_databaseName);
        static string _collectionName = "Kyruus";
        static DocumentClient _documentClient = new DocumentClient(new Uri(serviceEndPoint), authorizationKey);

        public static List<Provider> GetProviders()
        {
            Microsoft.Azure.Documents.DocumentCollection collection = _documentClient.CreateDocumentCollectionQuery(_databaseUri)
                .ToList()
                .First(c => c.Id == _collectionName);

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);

            string sql = "SELECT * FROM c ";
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

            List<Provider> providers = _documentClient.CreateDocumentQuery<Provider>(collectionUri, sql, options).ToList();

            return providers;
        }
    }
}
