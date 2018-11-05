using AzureSearch.Common;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
