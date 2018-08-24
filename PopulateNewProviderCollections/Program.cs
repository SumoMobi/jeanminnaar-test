using Microsoft.Azure.Documents;
using PopulateNewProviderCollections.BusinessRules;
using PopulateNewProviderCollections.DataAccess;
using PopulateNewProviderCollections.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace PopulateNewProviderCollections
{
    class Program
    {
        public static void Main(string[] args)
        {   //Go from synchronous to asynchronous
            MainAsync(args).GetAwaiter().GetResult();
            //Not needed on latest VS 2017 since it supports "static async Task Main(string[] args)".  Did not work for me.
        }

        public static async Task MainAsync(string[] args)
        {
            /*            List<DgCondition> conditions = DgConditionsCollectionDa.GetAll();
                        //get 25% list.  Assuming all is new.
                        StringBuilder ids = new StringBuilder();
                        int skip = 27;//newEntries.Count / (newEntries.Count / 100 * 25);
                        for (int i = 0; i < conditions.Count - skip; i += skip)
                        {
                            ids.Append($"'{conditions[i].id}',");
                        }

                        return;
                        await CreateCollections();
            */

            List<DgProvider> providers = DgProvidersCollectionDa.GetFiltered();

            /*
                        DgProvidersCollectionDa.CreateBackup(providers);
                        DgProvidersCollectionDa.PopulateFilteredCollection(providers);
            */
            await DgProvidersCollectionDa.PopulateNarrowCollection(providers);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
            return;

            //Need to get the filtered list before proceeding.
            providers = providers
                .Where(p => p.locations != null && p.locations.Length > 0 && string.Compare(p.show_in_pmc, "Yes", true) == 0)
                .ToList();
            List<DgCondition> newEntries;
            List<DgCondition> existingEntries;
            List<string> obsoleteEntries;
            DgConditionsBr.DetermineUpdates(providers, out newEntries, out existingEntries, out obsoleteEntries);

//            DgConditionsCollectionDa.Insert(newEntries);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private async static Task CreateCollections()
        {
            List<DocumentCollection> collections = BhProvidersDatabaseDa.ViewCollections();
            foreach (DocumentCollection collection in collections)
            {
                Console.WriteLine($"collection name = {collection.Id}");
            }

            DocumentCollection conditionsCollection = await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgProviderConditions);
            DocumentCollection insurancesCollection = await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgProviderInsurancesAccepted);
            DocumentCollection namesCollection = await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgProviderNames);
            DocumentCollection specialtiesCollection = await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgProviderSpecialties);
            DocumentCollection primaryCaresCollection = await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgProviderPrimarycarePhysicians);
            DocumentCollection filteredProviderCollection = await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgFilteredProviders);
            DocumentCollection backupProviderCollection = await BhProvidersDatabaseDa.CreateCollection(BhProvidersDatabaseDa.CollectionNames.DgBackupProviders);

            Console.WriteLine("Created new collections");
            collections = BhProvidersDatabaseDa.ViewCollections();
            foreach (DocumentCollection collection in collections)
            {
                Console.WriteLine($"collection name = {collection.Id}");
            }

        }
    }
}
