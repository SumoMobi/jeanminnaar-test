using System;
using System.Collections.Generic;
using System.Text;

namespace PopulateNewProviderCollections.DataModels
{
    public class DgCondition
    {
        public string id { get; set; }
        public string partitionKey { get; set; }
        public string[] scope_of_practice_terms { get; set; }
    }
}
