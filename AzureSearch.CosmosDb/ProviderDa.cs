using AzureSearch.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AzureSearch.CosmosDb
{
    public class ProviderDa
    {

        public static List<Provider> GetAllFromDownload()
        {
            string contents = File.ReadAllText(@"C:\temp\kyruusExtractWantedOnly.json");
            List<Provider> providers = JsonConvert.DeserializeObject<List<Provider>>(contents);
            return providers;
        }
    }
}
