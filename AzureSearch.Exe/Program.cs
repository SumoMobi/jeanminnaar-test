using System;
using System.Configuration;
using System.Net.Http;
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
                Console.WriteLine("\t ui (to upload the Azure Search index data)");
                Console.WriteLine("\t us (to upload the Azure Search suggestions data)");
                Console.WriteLine("Hit any key to continue");
                Console.Read();
            }
            string command = args[0];
            command = command.ToLower();
            Task task = null;
            switch (command)
            {
                case "ea":
                    task = Task.Run(() => Extract.Downloader.ExtractAll());
                    break;
                case "ewa":
                    task = Task.Run(() => Extract.Downloader.ExtractWantedOnly());
                    break;
                case "ui":
                    task = Task.Run(() => Search.Loader.Update());
                    break;
                case "us":
                    string apiKey = ConfigurationManager.AppSettings["AzureSearchApiKey"];
                    task = Task.Run(() => Suggestions.SpecialtiesLoader.Update(apiKey));
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
            }
            catch (Exception)
            {

            }
        }

    }
}
