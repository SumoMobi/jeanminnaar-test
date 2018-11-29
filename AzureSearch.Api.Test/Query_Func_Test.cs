using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AzureSearch.Api.Test
{
    [TestClass]
    public class Query_Func_Test
    {
        [TestMethod]
        public void AreParametersAcceptable_Test01()
        {
            List<System.Collections.Generic.KeyValuePair<string, string>> parms;
            bool result;
            List<string> failureReasons;

            parms = new List<KeyValuePair<string, string>>();
            result = Query_Func.AreParametersAcceptable(parms, out failureReasons);
            Assert.AreEqual(false, result);
            Assert.AreEqual(4, failureReasons.Count);
            Assert.AreEqual("No parameters provided.", failureReasons[0]);
            Assert.AreEqual("'skip' parameter is required.", failureReasons[1]);
            Assert.AreEqual("'take' parameter is required.", failureReasons[2]);
            Assert.AreEqual("Must specify at least one universal search or one filter parameter.", failureReasons[3]);

            parms.Add(new KeyValuePair<string, string>("skip", "0"));
            parms.Add(new KeyValuePair<string, string>("take", "25"));
            parms.Add(new KeyValuePair<string, string>("skip", "0"));
            parms.Add(new KeyValuePair<string, string>("unknown", "0"));
            parms.Add(new KeyValuePair<string, string>("filter", "cancer"));
            result = Query_Func.AreParametersAcceptable(parms, out failureReasons);

        }
        [TestMethod]
        public void QueryParmsToLower_Test01()
        {
            List<KeyValuePair<string, string>> parms;
            List<KeyValuePair<string, string>> results;

            parms = new List<KeyValuePair<string, string>>();
            results = Query_Func.TakeQueryParmsToLower(parms);
            Assert.AreEqual(0, results.Count);

            parms.Add(new KeyValuePair<string, string>("skip", ""));
            parms.Add(new KeyValuePair<string, string>("SkiP", "Ul"));
            parms.Add(new KeyValuePair<string, string>("filter", ""));
            parms.Add(new KeyValuePair<string, string>("filter", "Ul"));
            parms.Add(new KeyValuePair<string, string>("filter", "Ul:value"));
            parms.Add(new KeyValuePair<string, string>("fIlTer", "Ul:VaLue"));
            results = Query_Func.TakeQueryParmsToLower(parms);
            int i = 0;
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("skip", results[i].Key);
            Assert.AreEqual("", results[i].Value);
            i++;
            Assert.AreEqual("skip", results[i].Key);
            Assert.AreEqual("Ul", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Key);
            Assert.AreEqual("", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Key);
            Assert.AreEqual("Ul", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Key);
            Assert.AreEqual("ul:value", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Key);
            Assert.AreEqual("ul:VaLue", results[i].Value);

        }
        [TestMethod]
        public void GetFilters_Test01()
        {
            List<KeyValuePair<string, string>> parms;
            List<Filter> filters;

            //Parameters in this case has to contain valid filter entries lower cased already.

            parms = new List<KeyValuePair<string, string>>();
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(0, filters.Count);

            parms.Add(new KeyValuePair<string, string>("skip", "10"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(0, filters.Count);

            parms.Add(new KeyValuePair<string, string>("filter", "condition:canCer"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(1, filters.Count);
            int i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            int v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);

            parms.Add(new KeyValuePair<string, string>("take", "20"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(1, filters.Count);
            i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);

            parms.Add(new KeyValuePair<string, string>("filter", "ismale:true"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(2, filters.Count);
            i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);
            i++;
            Assert.AreEqual("isMale", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("true", filters[i].Values[v]);

            parms.Add(new KeyValuePair<string, string>("filter", "languages:Hindy"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(3, filters.Count);
            i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);
            i++;
            Assert.AreEqual("isMale", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("true", filters[i].Values[v]);
            i++;
            Assert.AreEqual("languages", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("Hindy", filters[i].Values[v]);

            parms.Add(new KeyValuePair<string, string>("filter", "languages:Spanish"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(3, filters.Count);
            i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);
            i++;
            Assert.AreEqual("isMale", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("true", filters[i].Values[v]);
            i++;
            Assert.AreEqual("languages", filters[i].AzureIndexFieldName);
            Assert.AreEqual(2, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("Hindy", filters[i].Values[v]);
            v++;
            Assert.AreEqual("Spanish", filters[i].Values[v]);

            parms.Add(new KeyValuePair<string, string>("filter", "languages:Russian"));
            parms.Add(new KeyValuePair<string, string>("filter", "agesseen:babies"));
            parms.Add(new KeyValuePair<string, string>("filter", "agesseen:adults"));
            parms.Add(new KeyValuePair<string, string>("filter", "acceptedinsurances:AETNA"));
            parms.Add(new KeyValuePair<string, string>("filter", "acceptedinsurances:United"));
            parms.Add(new KeyValuePair<string, string>("filter", "acceptnewpatients:true"));
            parms.Add(new KeyValuePair<string, string>("filter", "providertype:vet"));
            parms.Add(new KeyValuePair<string, string>("filter", "providertype:vet2"));
            parms.Add(new KeyValuePair<string, string>("filter", "networkaffiliations:ABC"));
            parms.Add(new KeyValuePair<string, string>("filter", "networkaffiliations:DEF"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(8, filters.Count);
            i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);
            i++;
            Assert.AreEqual("isMale", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("true", filters[i].Values[v]);
            i++;
            Assert.AreEqual("languages", filters[i].AzureIndexFieldName);
            Assert.AreEqual(3, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("Hindy", filters[i].Values[v]);
            v++;
            Assert.AreEqual("Spanish", filters[i].Values[v]);
            v++;
            Assert.AreEqual("Russian", filters[i].Values[v]);
            i++;
            Assert.AreEqual("agesSeen", filters[i].AzureIndexFieldName);
            Assert.AreEqual(2, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("babies", filters[i].Values[v]);
            v++;
            Assert.AreEqual("adults", filters[i].Values[v]);
            i++;
            Assert.AreEqual("acceptedInsurances", filters[i].AzureIndexFieldName);
            Assert.AreEqual(2, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("AETNA", filters[i].Values[v]);
            v++;
            Assert.AreEqual("United", filters[i].Values[v]);
            i++;
            Assert.AreEqual("acceptNewPatients", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("true", filters[i].Values[v]);
            i++;
            Assert.AreEqual("providerType", filters[i].AzureIndexFieldName);
            Assert.AreEqual(2, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("vet", filters[i].Values[v]);
            v++;
            Assert.AreEqual("vet2", filters[i].Values[v]);
            i++;
            Assert.AreEqual("networkAffiliations", filters[i].AzureIndexFieldName);
            Assert.AreEqual(2, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("ABC", filters[i].Values[v]);
            v++;
            Assert.AreEqual("DEF", filters[i].Values[v]);

        }

        [TestMethod]
        public void GetParms_Test01()
        {
            List<KeyValuePair<string, string>> parms;
            List<KeyValuePair<string, string>> results;
            int skip; int take; string seed; string universal; List<Filter> filters;

            //Parameters in this case has to be a valid set.

            parms = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("skip", "5"),
                new KeyValuePair<string, string>("take", "10"),
                new KeyValuePair<string, string>("universal", "15"),
            };

            Query_Func.GetParameters(parms, out skip, out take, out seed, out universal, out filters);
            Assert.AreEqual(5, skip);
            Assert.AreEqual(10, take);
            Assert.AreEqual("15", universal);
            Assert.AreEqual(null, seed);
            Assert.AreEqual(0, filters.Count);

            parms = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("skip", "5"),
                new KeyValuePair<string, string>("take", "10"),
                new KeyValuePair<string, string>("seed", "15"),
                new KeyValuePair<string, string>("filter", "condition:cancer"),
                new KeyValuePair<string, string>("filter", "languages:Hindy"),
                new KeyValuePair<string, string>("filter", "languages:English")
            };

            Query_Func.GetParameters(parms, out skip, out take, out seed, out universal, out filters);
            Assert.AreEqual(5, skip);
            Assert.AreEqual(10, take);
            Assert.AreEqual(null, universal);
            Assert.AreEqual("15", seed);
            Assert.AreEqual(2, filters.Count);
            int i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            int v = 0;
            Assert.AreEqual("cancer", filters[i].Values[v]);
            i++;
            Assert.AreEqual("languages", filters[i].AzureIndexFieldName);
            Assert.AreEqual(2, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("Hindy", filters[i].Values[v]);
            v++;
            Assert.AreEqual("English", filters[i].Values[v]);

        }
    }
}
