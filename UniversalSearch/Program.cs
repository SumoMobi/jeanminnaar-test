using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
namespace UniversalSearch
{
    class Program
    {
        private static string _dbName = "bhprovidersdb";
        private static string _collectionName = "DGProviders";
        private static string _endpoint = "https://bh-dg-dev-provider-db-cosmos.documents.azure.com:443/";
        private static string _primaryKey = "O1H3t2ngQvqd9VHv8Y3Rz2KI6sShAwNArYqOTLOw3f2Or4ELOpVcdZLnurZO5w3Fj9YIcur4Y57ah9F9W4xPmQ==";
        static void Main(string[] args)
        {
            string sql;
            string whereClause;
            string searchTerm = "Nicole";

            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] searchWords = searchTerm.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName);
            DocumentClient dbClient = new DocumentClient(new Uri(_endpoint), _primaryKey);

            //Run the Name Search.  If the user provided "joe smith", we need to look for "joe" in first and last name and same with "smith"
            whereClause = string.Empty;
            if (searchWords.Length > 1)
            {   //What if the user provided more than two words?  Probably won't find a name match.
                foreach (string searchWord in searchWords)
                {
                    whereClause += $"(STARTSWITH(c.name.first_name, '{searchWord}') OR STARTSWITH(c.name.last_name, '{searchWord}')) AND ";
                }
                //This will be something like: 
                //      (STARTSWITH(c.lower_first_name, 'joe') OR STARTSWITH(c.lower_last_name, 'joe')) AND
                //      (STARTSWITH(c.lower_first_name, 'smith') OR STARTSWITH(c.lower_last_name, 'smith')) AND
                //If we have:
                //      1   Joe Smith
                //      2   Joe Blow
                //      3   Jay Smith
                //we should only get back 1.
                whereClause = whereClause.Substring(0, whereClause.Length - 5);
            }
            else
            {
                whereClause = $"STARTSWITH(c.name.first_name, '{searchWords[0]}') OR STARTSWITH(c.name.last_name, '{searchWords[0]}')";
            }
            sql = $"SELECT VALUE c.id FROM c WHERE {whereClause}";
            List<Task> tasks = new List<Task>();
            ConcurrentBag<string> ids = new ConcurrentBag<string>();
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            tasks.Add(Task.Run(() =>
            {
                List<string> query = dbClient.CreateDocumentQuery<string>(collectionUri, sql, options).ToList();
                foreach (string id in query)
                {
                    ids.Add(id);
                }
            }));
//            List<dynamic> query = dbClient.CreateDocumentQuery<dynamic>(collectionUri, sql, options).ToList();

            //Run the specialties search.

            Task.WaitAll(tasks.ToArray());

        }
    }
}
