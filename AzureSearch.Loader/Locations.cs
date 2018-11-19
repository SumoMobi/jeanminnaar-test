using AzureSearch.Common;
using AzureSearch.CosmosDb;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.Loader
{
    public class Location
    {
        public string id { get; set; }
        public GeographyPoint coordinates { get; set; }
    }
    public class LocationComparer : IEqualityComparer<Location>
    {
        public bool Equals(Location left, Location right)
        {
            if (left.id == right.id)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Location obj)
        {
            int hash = 0;
            for (int c = 0; c < obj.id.Length; c++)
            {
                hash += obj.id[c];
            }
            return hash;
        }
    }
    public class Locations
    {
        public static void Upload(string apiKey, string serviceName)
        {
            //Get all the provider data.
            DateTime startDateTime = DateTime.Now;
            List<KyruusDataStructure> providers = ProviderDa.GetAllFromDownload();
            Console.WriteLine($"{providers.Count} providers fetched.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            startDateTime = DateTime.Now;
            List<Location> locations = new List<Location>();
            foreach (KyruusDataStructure p in providers)
            {
                foreach (Common.Location l in p.locations)
                {
                    if (l.coordinates == null)
                    {
                        continue;
                    }
                    locations.Add(new Location
                    {
                        coordinates = GeographyPoint.Create(l.coordinates.lat, l.coordinates.lon),
                        id = l.id  //Kyruus has multiple 'id's for the same external_id.  But if we use the external_id and that changes (planned to switch over to Cerner's ID),
                                   //we have to rebuild both locations and providers indexes and coordinate such with Sitecore releases.  Can't use coordinates either to ID a facility 
                                   //since that can change slightly too.  Keep in mind that we need to attach the IDs to the providers too...
                    });
                }
            }
            Console.WriteLine($"{providers.Count} providers.  {locations.Count} locations.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Get unique list of locations.
            startDateTime = DateTime.Now;
            locations = locations.Distinct(new LocationComparer()).ToList();
            Console.WriteLine($"{locations.Count} unique locations.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Upload the data to Azure Search
            startDateTime = DateTime.Now;
            IndexLoader.MergeOrUpload(locations, apiKey, serviceName, "locations");
            Console.WriteLine($"Index uploaded.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

        }
    }
}
