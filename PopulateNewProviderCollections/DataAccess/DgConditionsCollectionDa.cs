using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using PopulateNewProviderCollections.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopulateNewProviderCollections.DataAccess
{
    public class DgConditionsCollectionDa
    {

        private static Uri collectionUri => UriFactory.CreateDocumentCollectionUri
            (BhProvidersDatabaseDa.DatabaseName, BhProvidersDatabaseDa.CollectionNames.DgProviderConditions.ToString());
        /// <summary>
        /// Gets everything that is in the conditions collection.
        /// </summary>
        /// <returns></returns>
        public static List<DgCondition> GetAll()
        {
            string sql = "SELECT c.id, c.partitionKey, c.scope_of_practice_terms FROM c";
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

            List<DgCondition> conditions = BhProvidersDatabaseDa.DocumentClient.CreateDocumentQuery<DgCondition>(collectionUri, sql, options).ToList();
            return conditions;
        }

        public static void Insert(List<DgCondition> dgConditions)
        {
            foreach (DgCondition dgCondition in dgConditions)
            {
                BhProvidersDatabaseDa.DocumentClient.CreateDocumentAsync(collectionUri, dgCondition);
            }
        }

        public static void Update(List<DgCondition> providerConditions)
        {


        }

        public static void Delete(List<string> ids)
        {

        }
    }
}
