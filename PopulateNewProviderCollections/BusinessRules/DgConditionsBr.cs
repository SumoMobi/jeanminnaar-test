using PopulateNewProviderCollections.DataAccess;
using PopulateNewProviderCollections.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PopulateNewProviderCollections.BusinessRules
{
    public class DgConditionsBr
    {
        public static void DetermineUpdates(List<DgProvider> providers, out List<DgCondition> newEntries, out List<DgCondition> existingEntries, out List<string> obsoleteEntries)
        {
            //Extract the conditions from the big (source) list.
            List<DgCondition> sourceEntries = providers
                .Select(p => new DgCondition
                {
                    id = p.id,
                    partitionKey = p.id,
                    scope_of_practice_terms = p.scope_of_practice_terms
                })
                .Where(p => p.scope_of_practice_terms != null && p.scope_of_practice_terms.Length != 0)
                .ToList();

            //Get what we have in the relevant subset collection in Cosmos DB.
            List<DgCondition> conditions = DgConditionsCollectionDa.GetAll();

            //See what needs to be added to the relevant subset.
            newEntries = sourceEntries.Except(conditions).ToList();

            //See what needs to be deleted from the relevant subset.
            List<DgCondition> obsoleteEntriesAll = conditions.Except(sourceEntries).ToList();
            obsoleteEntries = obsoleteEntriesAll.Select(c => c.id).ToList();

            //remove inserts and deletes from the source list.
            existingEntries = sourceEntries.Except(newEntries).ToList();
            existingEntries = existingEntries.Except(obsoleteEntriesAll).ToList();

        }
    }
}
