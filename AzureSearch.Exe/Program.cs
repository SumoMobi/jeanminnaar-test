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
                Console.WriteLine("Provide one of the following commands:");
                Console.WriteLine("\t ea (to extract all kyruus data to disk)");
                Console.WriteLine("\t ewa (to extract just those kyruus providers we want)");
                Console.WriteLine("\t ul (to upload the Azure Search locations data)");
                Console.WriteLine("\t up (to upload the Azure Search's providers index)");
                Console.WriteLine("\t us (to upload the Azure Search specialties data)");
                Console.WriteLine("\t uc (to upload the Azure Search conditions data)");
                Console.WriteLine("\t ui (to upload the Azure Search insurances data)");
                Console.WriteLine("\t un (to upload the Azure Search names data)");
                Console.WriteLine("Hit any key to continue");
                Console.Read();
            }
            string apiKey = ConfigurationManager.AppSettings["AzureSearchApiKey"];
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            string command = args[0];
            command = command.ToLower();
            Task task = null;
            switch (command)
            {
                case "ea":
                    task = Task.Run(() => AzureSearch.Source.Kyruus.ExtractAll());
                    break;
                case "ewa":
                    task = Task.Run(() => AzureSearch.Source.Kyruus.ExtractWantedOnly());
                    break;
                case "ul":
                    task = Task.Run(() => AzureSearch.Loader.Locations.Upload(apiKey, serviceName));
                    break;
                case "up":
                    task = Task.Run(() => AzureSearch.Loader.Providers.Upload(apiKey, serviceName));
                    break;
                case "us":
                    task = Task.Run(() => AzureSearch.Loader.Specialties.Upload(apiKey, serviceName));
                    break;
                case "uc":
                    task = Task.Run(() => AzureSearch.Loader.Conditions.Upload(apiKey, serviceName));
                    break;
                case "ui":
                    task = Task.Run(() => AzureSearch.Loader.Insurances.Upload(apiKey, serviceName));
                    break;
                case "un":
                    task = Task.Run(() => AzureSearch.Loader.Names.Upload(apiKey, serviceName));
                    break;
                default:
                    Console.WriteLine("Provide one of the following commands:");
                    Console.WriteLine("\t ea (to extract all kyruus data to disk)");
                    Console.WriteLine("\t ewa (to extract just those kyruus providers we want)");
                    Console.WriteLine("\t ui (to upload the Azure Search index data)");
                    Console.WriteLine("Hit any key to continue");
                    Console.Read();
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

    }
}
