using AzureSearch.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureSearch.Upload
{
    public class Loader
    {
        public static async Task UpdateIndex()
        {

        }

        public static void UpdateSuggestions()
        {
            List<Provider> providers = ProviderDa.GetProviders();
        }
    }
}
