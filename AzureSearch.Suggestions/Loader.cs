using AzureSearch.Common;
using AzureSearch.CosmosDb;
using System.Collections.Generic;

namespace AzureSearch.Suggestions
{
    public class Loader
    {
        public static void Update()
        {
            //Get all the provider data.
            List<Provider> providers = ProviderDa.GetAllFromDownload();

            //Drop and recreate the Azure Index for suggestions.

            //Load the suggestions index.

        }
    }
}
