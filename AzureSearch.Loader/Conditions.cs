using AzureSearch.Common;
using AzureSearch.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.Loader
{
    public class Conditions
    {
        public static void Upload(string apiKey, string serviceName)
        {
            DateTime startDateTime = DateTime.Now;

            //Get all the provider data.
            List<KyruusDataStructure> providers = ProviderDa.GetAllFromDownload();
            Console.WriteLine($"{providers.Count} providers fetched.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Get the complete list of primary care and non-primary care conditions.
            startDateTime = DateTime.Now;
            List<string> primaryCareConditions;
            List<string> nonPrimaryCareConditions;
            GetConditions(providers, out primaryCareConditions, out nonPrimaryCareConditions);
            Console.WriteLine($"{primaryCareConditions.Count} primaryCareConditions.  {nonPrimaryCareConditions.Count} nonPrimaryCareConditions.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //De-dupe the primary and non primary care conditions.
            startDateTime = DateTime.Now;
            primaryCareConditions = primaryCareConditions
                .Where(p => string.IsNullOrWhiteSpace(p) == false)
                .Distinct()
                .ToList();
            nonPrimaryCareConditions = nonPrimaryCareConditions
                .Where(n => string.IsNullOrWhiteSpace(n) == false)
                .Distinct()
                .ToList();
            //Put the two condition types together.
            List<ConditionIndexDataStructure> conditions = primaryCareConditions
                .Select(p => new ConditionIndexDataStructure
                {
                    condition = p,
                    id = string.Empty,
                    isPrimaryCare = true
                })
                .ToList();
            conditions.AddRange(nonPrimaryCareConditions
                .Select(n => new ConditionIndexDataStructure
                {
                    condition = n,
                    id = string.Empty,
                    isPrimaryCare = false
                })
                .OrderBy(c => c.condition)
                .ThenByDescending(c => c.isPrimaryCare)
                .ToList());
            //Note that we will load this index in a reasonable order so as to avoid index fragmentation and speed up inquiry times.
            //Now assign the ID
            for (int c = 0; c < conditions.Count; c++)
            {
                conditions[c].id = (c + 1).ToString();
            }
            Console.WriteLine($"{primaryCareConditions.Count} unique primaryCareConditions.  {nonPrimaryCareConditions.Count} unique nonPrimaryCareConditions.  {conditions.Count} combined conditions.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Drop and recreate the Azure Index.
            startDateTime = DateTime.Now;
            RecreateIndex();
            Console.WriteLine($"Recreate index response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Populate conditions index.
            startDateTime = DateTime.Now;
            IndexLoader.MergeOrUpload(conditions, apiKey, serviceName, "conditions");
            Console.WriteLine($"Index upload response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

        }

        private static void GetConditions(List<KyruusDataStructure> providers, out List<string> primaryCareConditions, out List<string> nonPrimaryCareConditions)
        {
            //This is what the Kyruus data looks like:
            //"scope_of_practice": {
            //    "concepts": [
    		//		{
			//		"attributes": [
    		//				"pci:Clinical Interest",
    		//				"condition",
    		//				"primary care interest"
			//		],
			//		"cui": "C3657846",
			//		"name": "geriatrics",
			//		"searchability": "searchable",
			//		"terms": [
			//			{
			//				"name": "geriatrics",
			//				"tui": "T00019523"
            //
            //            },
			//			{
			//				"name": "elder care",
			//				"tui": "T00019524"
			//			},


            primaryCareConditions = new List<string>();
            nonPrimaryCareConditions = new List<string>();
            foreach (KyruusDataStructure p in providers)
            {
                if (p.scope_of_practice == null)
                {
                    continue;
                }
                if (p.scope_of_practice.concepts == null || p.scope_of_practice.concepts.Length == 0)
                {
                    continue;
                }
                foreach(Concept c in p.scope_of_practice.concepts)
                {
                    if (c.terms == null || c.terms.Length == 0)
                    {
                        continue;
                    }
                    if (c.searchability != "searchable")
                    {
                        continue;
                    }
                    bool isPrimaryCare = false;
                    if (c.attributes != null && c.attributes.Contains("primary care"))
                    {
                        isPrimaryCare = true;
                    }
                    if (isPrimaryCare)
                    {
                        primaryCareConditions.AddRange(c.terms.Select(t => t.name));
                    }
                    else
                    {
                        nonPrimaryCareConditions.AddRange(c.terms.Select(t => t.name));
                    }
                }
            }
        }

        private static void RecreateIndex()
        {
            Console.WriteLine("First need to drop and create the index.  Hit ENTER when done.");
            Console.ReadLine();
            //Drop the indexes
            //TODO.  For now do it through Postman.
            //Create the indexes
            //TODO.  For now do it through Postman.
        }

    }
}
