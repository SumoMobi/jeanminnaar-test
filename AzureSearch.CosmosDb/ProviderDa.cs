using AzureSearch.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AzureSearch.CosmosDb
{
    public class ProviderDa
    {

        public static List<KyruusDataStructure> GetAllFromDownload()
        {
            string contents = File.ReadAllText(@"C:\temp\kyruusExtractWantedOnly.json");
            List<KyruusDataStructure> providers = JsonConvert.DeserializeObject<List<KyruusDataStructure>>(contents);
            return providers;
        }
    }
}
