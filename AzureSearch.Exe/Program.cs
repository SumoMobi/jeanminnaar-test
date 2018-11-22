using System;
using System.Configuration;
using System.Threading.Tasks;

namespace AzureSearch.Exe
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 1)
            {
                ShowHelp();
                return;
            }
            string apiKey = ConfigurationManager.AppSettings["AzureSearchApiKey"];
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            string storageAccountName = ConfigurationManager.AppSettings["storageAccountName"];
            string storageAccountKey = ConfigurationManager.AppSettings["storageAccountKey"];
            string Dev4CosmosKey = ConfigurationManager.AppSettings["Dev4CosmosKey"];
            string Dev4CosmosUrl = ConfigurationManager.AppSettings["Dev4CosmosUrl"];

            string command = args[0];
            command = command.ToLower();
            Task task = null;
            switch (command)
            {
                case "ea":
                    task = Task.Run(() => Source.Kyruus.ExtractAll());
                    break;
                case "ewa":
                    task = Task.Run(() => Source.Kyruus.ExtractWantedOnly());
                    break;
                case "ul":
                    task = Task.Run(() => Loader.Locations.Upload(apiKey, serviceName));
                    break;
                case "up":
                    task = Task.Run(() => Loader.Providers.Upload(apiKey, serviceName));
                    break;
                case "us":
                    task = Task.Run(() => Loader.Specialties.Upload(apiKey, serviceName));
                    break;
                case "uc":
                    task = Task.Run(() => Loader.Conditions.Upload(apiKey, serviceName));
                    break;
                case "ui":
                    task = Task.Run(() => Loader.Insurances.Upload(apiKey, serviceName));
                    break;
                case "un":
                    task = Task.Run(() => Loader.Names.Upload(apiKey, serviceName));
                    break;
                case "pca":
                    Performance.CosmosDb.GetDocuments(Dev4CosmosUrl, Dev4CosmosKey);    //3.5 seconds
                    break;
                case "pcs":
                    Performance.CosmosDb.GetDocumentsSpecificElements(Dev4CosmosUrl, Dev4CosmosKey);    //3.4 seconds
                    break;
                case "pcss":
                    task = Task.Run(() => Performance.CosmosDb.GetDocumentsStoredProcedure(Dev4CosmosUrl, Dev4CosmosKey));    //3.0 seconds
                    break;
                case "pcssp":
                    task = Task.Run(() => Performance.CosmosDb.GetDocumentsSpecificElementsInParallel(Dev4CosmosUrl, Dev4CosmosKey));    //4.6 seconds
                    break;
                case "pb":
                    Performance.BlobStorage.GetDocuments(storageAccountKey, storageAccountName);    //7.0 seconds.  Goes up to 12 seconds at times.
                    break;
                case "pbp":
                    Performance.BlobStorage.GetDocumentsInParallel(storageAccountKey, storageAccountName);    //4.0 seconds.  Goes up to 26 seconds at times.
                    break;
                case "pbn":
                    Performance.BlobStorageNarrow.GetDocuments(storageAccountKey, storageAccountName);
                    break;
                case "pbpn":
                    Performance.BlobStorageNarrow.GetDocumentsInParallel(storageAccountKey, storageAccountName);
                    break;
                default:
                    ShowHelp();
                    break;
            }
            try
            {
                if (task != null)
                {
                    task.Wait();
                }
                Console.WriteLine("Hit ENTER to continue.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ".  Hit ENTER to continue.");
                Console.ReadLine();
            }
        }
        static void ShowHelp()
        {
            Console.WriteLine("Provide one of the following commands:");
            Console.WriteLine("\t ea (to extract all kyruus data to disk)");
            Console.WriteLine("\t ewa (to extract just those kyruus providers we want)");
            Console.WriteLine("\t ul (to upload the Azure Search locations data)");
            Console.WriteLine("\t up (to upload the Azure Search's providers index)");
            Console.WriteLine("\t us (to upload the Azure Search specialties data)");
            Console.WriteLine("\t uc (to upload the Azure Search conditions data)");
            Console.WriteLine("\t ui (to upload the Azure Search insurances data)");
            Console.WriteLine("\t un (to upload the Azure Search names data)");
            Console.WriteLine("\t pca (performance against Cosmos bring back all nodes)");
            Console.WriteLine("\t pcs (performance against Cosmos bring back select nodes)");
            Console.WriteLine("\t pcss (performance against Cosmos bring back select nodes using stored procedure)");
            Console.WriteLine("\t pcssp (performance against Cosmos bring back select nodes using 5 threads in parallel)");
            Console.WriteLine("\t pb (performance against Blob Storage)");
            Console.WriteLine("\t pbp (performance against Blob Storage using 5 threads in parallel)");
            Console.WriteLine("Hit ENTER to continue");
            Console.ReadLine();
        }

    }
}
