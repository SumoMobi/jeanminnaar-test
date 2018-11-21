using AzureSearch.Common;
using AzureSearch.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.Loader
{
    public class Names
    {
        public static void Upload(string apiKey, string serviceName)
        {
            DateTime startDateTime = DateTime.Now;

            //Get all the provider data.
            List<KyruusDataStructure> providers = ProviderDa.GetAllFromDownload();
            Console.WriteLine($"{providers.Count} providers fetched.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Get the complete list of names.
            startDateTime = DateTime.Now;
            List<string> firstAndLastNames = new List<string>();
            foreach(KyruusDataStructure p in providers)
            {
                if (p.name == null)
                {
                    continue;
                }
                string firstName = string.Empty;
                string lastName = string.Empty;
                string firstAndLastName = string.Empty;
                //p.name.full_name does not have the p.preferred_name in the first name part.
                if (string.IsNullOrWhiteSpace(p.preferred_name))
                {
                    firstName = p.name.first_name;
                }
                else
                {
                    firstName = p.preferred_name;
                }
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    firstName = string.Empty;
                }
                lastName = p.name.last_name;
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    lastName = string.Empty;
                }
                if (firstName.Length == 0)
                {
                    firstAndLastName = lastName;
                }
                else
                {
                    firstAndLastName = firstName + " " + lastName;
                }
                firstAndLastNames.Add(firstAndLastName);
            }
            Console.WriteLine($"{firstAndLastNames.Count} names.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //De-dupe the names.
            startDateTime = DateTime.Now;
            firstAndLastNames = firstAndLastNames
                .Where(p => string.IsNullOrWhiteSpace(p) == false)
                .Distinct()
                .ToList();
            //Now assign the ID
            NameIndexDataStructure[] names = new NameIndexDataStructure[firstAndLastNames.Count];
            for (int n = 0; n < firstAndLastNames.Count; n++)
            {
                names[n] = new NameIndexDataStructure { firstAndLastName = firstAndLastNames[n], id = (n + 1).ToString() };
            }
            Console.WriteLine($"{names.Length} names to be uploaded.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Drop and recreate the Azure Index.
            startDateTime = DateTime.Now;
            RecreateIndex();
            Console.WriteLine($"Recreate index response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Populate index.
            startDateTime = DateTime.Now;
            IndexLoader.MergeOrUpload(names.ToList(), apiKey, serviceName, "names");
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
