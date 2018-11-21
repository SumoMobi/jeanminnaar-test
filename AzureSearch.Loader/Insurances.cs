using AzureSearch.Common;
using AzureSearch.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.Loader
{
    public class Insurances
    {
        public static void Upload(string apiKey, string serviceName)
        {
            DateTime startDateTime = DateTime.Now;

            //Get all the provider data.
            List<KyruusDataStructure> providers = ProviderDa.GetAllFromDownload();
            Console.WriteLine($"{providers.Count} providers fetched.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Get the complete list of insurances.
            startDateTime = DateTime.Now;
            List<string> insurances = new List<string>();
            foreach (KyruusDataStructure p in providers)
            {
                if (p.insurance_accepted == null || p.insurance_accepted.Length == 0)
                {
                    continue;
                }
                insurances.AddRange(p.insurance_accepted.Select(i => i.name));
            }
            Console.WriteLine($"{insurances.Count} insurances.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //De-dupe the insurances.
            startDateTime = DateTime.Now;
            insurances = insurances
                .Where(i => string.IsNullOrWhiteSpace(i) == false)
                .Distinct()
                .ToList();
            //Now assign the ID
            InsuranceIndexDataStructure[] insurancesIndexList = new InsuranceIndexDataStructure[insurances.Count];
            for (int i = 0; i < insurances.Count; i++)
            {
                insurancesIndexList[i] = new InsuranceIndexDataStructure { id = (i + 1).ToString(), insurance = insurances[i] };
            }
            Console.WriteLine($"{insurancesIndexList.Length} insurances to upload.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Drop and recreate the Azure Index.
            startDateTime = DateTime.Now;
            RecreateIndex();
            Console.WriteLine($"Recreate index response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Populate index.
            startDateTime = DateTime.Now;
            IndexLoader.MergeOrUpload(insurancesIndexList.ToList(), apiKey, serviceName, "insurances");
            Console.WriteLine($"Index upload response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

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
