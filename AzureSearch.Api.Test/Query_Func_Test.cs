using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AzureSearch.Api.Test
{
    [TestClass]
    public class Query_Func_Test
    {
        [TestMethod]
        public void AreParametersAcceptable_Test01()
        {
            List<NameValue> parms;
            bool result;
            List<string> failureReasons;

            parms = new List<NameValue>();
            result = Query_Func.AreParametersAcceptable(parms, out failureReasons);
            Assert.AreEqual(false, result);
            Assert.AreEqual(4, failureReasons.Count);
            Assert.AreEqual("No parameters provided.", failureReasons[0]);
            Assert.AreEqual("'skip' parameter is required.", failureReasons[1]);
            Assert.AreEqual("'take' parameter is required.", failureReasons[2]);
            Assert.AreEqual("Must specify at least one universal search or one filter parameter.", failureReasons[3]);

            parms.Add(new NameValue("skip", "0"));
            parms.Add(new NameValue("take", "25"));
            parms.Add(new NameValue("skip", "0"));
            parms.Add(new NameValue("unknown", "0"));
            parms.Add(new NameValue("filter", "cancer"));
            result = Query_Func.AreParametersAcceptable(parms, out failureReasons);

        }
        [TestMethod]
        public void QueryParmsToLower_Test01()
        {
            List<NameValue> parms;
            List<NameValue> results;

            parms = new List<NameValue>();
            results = Query_Func.TakeQueryParmsToLower(parms);
            Assert.AreEqual(0, results.Count);

            parms.Add(new NameValue("skip", ""));
            parms.Add(new NameValue("SkiP", "Ul"));
            parms.Add(new NameValue("filter", ""));
            parms.Add(new NameValue("filter", "Ul"));
            parms.Add(new NameValue("filter", "Ul:value"));
            parms.Add(new NameValue("fIlTer", "Ul:VaLue"));
            results = Query_Func.TakeQueryParmsToLower(parms);
            int i = 0;
            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("skip", results[i].Name);
            Assert.AreEqual("", results[i].Value);
            i++;
            Assert.AreEqual("skip", results[i].Name);
            Assert.AreEqual("Ul", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Name);
            Assert.AreEqual("", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Name);
            Assert.AreEqual("Ul", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Name);
            Assert.AreEqual("ul:value", results[i].Value);
            i++;
            Assert.AreEqual("filter", results[i].Name);
            Assert.AreEqual("ul:VaLue", results[i].Value);

        }
        [TestMethod]
        public void GetFilters_Test01()
        {
            List <NameValue> parms;
            List<Filter> filters;

            //Parameters in this case has to contain valid filter entries lower cased already.

            parms = new List<NameValue>();
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(0, filters.Count);

            parms.Add(new NameValue("skip", "10" ));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(0, filters.Count);

            parms.Add(new NameValue("filter", "condition_specialist:canCer"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(1, filters.Count);
            int i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            int v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);

            parms.Add(new NameValue("take", "20"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(1, filters.Count);
            i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("canCer", filters[i].Values[v]);

            parms.Add(new NameValue("filter", "ismale:true"));
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

            parms.Add(new NameValue("filter", "languages:Hindy"));
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

            parms.Add(new NameValue("filter", "languages:Spanish"));
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

            parms.Add(new NameValue("filter", "condition_primarycare:CANCER"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(4, filters.Count);
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
            i++;
            Assert.AreEqual("isPrimaryCare", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("true", filters[i].Values[v]);

            parms.Add(new NameValue("filter", "languages:Russian"));
            parms.Add(new NameValue("filter", "agesseen:babies"));
            parms.Add(new NameValue("filter", "agesseen:adults"));
            parms.Add(new NameValue("filter", "acceptedinsurances:AETNA"));
            parms.Add(new NameValue("filter", "acceptedinsurances:United"));
            parms.Add(new NameValue("filter", "acceptnewpatients:true"));
            parms.Add(new NameValue("filter", "providertypes:vet"));
            parms.Add(new NameValue("filter", "providertypes:vet2"));
            parms.Add(new NameValue("filter", "networkaffiliations:ABC"));
            parms.Add(new NameValue("filter", "networkaffiliations:DEF"));
            filters = Query_Func.GetFilters(parms);
            Assert.AreEqual(9, filters.Count);
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
            Assert.AreEqual("isPrimaryCare", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("true", filters[i].Values[v]);
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
            List<NameValue> parms;
            int skip; int take; string seed; string universal; List<Filter> filters; bool includeFacets; bool includeTotalCount;

            //Parameters in this case has to be a valid set.

            parms = new List<NameValue>()
            {
                new NameValue("skip", "5"),
                new NameValue("take", "10"),
                new NameValue("universal", "15"),
            };

            Query_Func.GetParameters(parms, out skip, out take, out seed, out universal, out filters, out includeFacets, out includeTotalCount);
            Assert.AreEqual(5, skip);
            Assert.AreEqual(10, take);
            Assert.AreEqual("15", universal);
            Assert.AreEqual(null, seed);
            Assert.AreEqual(0, filters.Count);
            Assert.AreEqual(false, includeFacets);
            Assert.AreEqual(false, includeTotalCount);

            parms = new List<NameValue>()
            {
                new NameValue("skip", "5"),
                new NameValue("take", "10"),
                new NameValue("seed", "15"),
                new NameValue("filter", "condition_specialist:Cancer"),
                new NameValue("filter", "languages:Hindy"),
                new NameValue("filter", "languages:English")
            };

            Query_Func.GetParameters(parms, out skip, out take, out seed, out universal, out filters, out includeFacets, out includeTotalCount);
            Assert.AreEqual(5, skip);
            Assert.AreEqual(10, take);
            Assert.AreEqual(null, universal);
            Assert.AreEqual("15", seed);
            Assert.AreEqual(2, filters.Count);
            Assert.AreEqual(false, includeFacets);
            Assert.AreEqual(false, includeTotalCount);
            int i = 0;
            Assert.AreEqual("conditions", filters[i].AzureIndexFieldName);
            Assert.AreEqual(1, filters[i].Values.Count);
            int v = 0;
            Assert.AreEqual("Cancer", filters[i].Values[v]);
            i++;
            Assert.AreEqual("languages", filters[i].AzureIndexFieldName);
            Assert.AreEqual(2, filters[i].Values.Count);
            v = 0;
            Assert.AreEqual("Hindy", filters[i].Values[v]);
            v++;
            Assert.AreEqual("English", filters[i].Values[v]);

            parms = new List<NameValue>()
            {
                new NameValue("skip", "5"),
                new NameValue("take", "10"),
                new NameValue("seed", "15"),
                new NameValue("filter", "condition_specialist:Cancer"),
                new NameValue("filter", "languages:Hindy"),
                new NameValue("filter", "languages:English"),
                new NameValue("includefacets", "true")
            };

            Query_Func.GetParameters(parms, out skip, out take, out seed, out universal, out filters, out includeFacets, out includeTotalCount);
            Assert.AreEqual(5, skip);
            Assert.AreEqual(10, take);
            Assert.AreEqual(null, universal);
            Assert.AreEqual("15", seed);
            Assert.AreEqual(2, filters.Count);
            Assert.AreEqual(true, includeFacets);
            Assert.AreEqual(false, includeTotalCount);

            parms = new List<NameValue>()
            {
                new NameValue("skip", "5"),
                new NameValue("take", "10"),
                new NameValue("seed", "15"),
                new NameValue("filter", "condition_specialist:Cancer"),
                new NameValue("filter", "languages:Hindy"),
                new NameValue("filter", "languages:English"),
                new NameValue("includetotalcount", "true")
            };

            Query_Func.GetParameters(parms, out skip, out take, out seed, out universal, out filters, out includeFacets, out includeTotalCount);
            Assert.AreEqual(5, skip);
            Assert.AreEqual(10, take);
            Assert.AreEqual(null, universal);
            Assert.AreEqual("15", seed);
            Assert.AreEqual(2, filters.Count);
            Assert.AreEqual(false, includeFacets);
            Assert.AreEqual(true, includeTotalCount);

            parms = new List<NameValue>()
            {
                new NameValue("skip", "5"),
                new NameValue("take", "10"),
                new NameValue("seed", "15"),
                new NameValue("filter", "condition_specialist:Cancer"),
                new NameValue("filter", "languages:Hindy"),
                new NameValue("filter", "languages:English"),
                new NameValue("includefacets", "true"),
                new NameValue("includetotalcount", "true")
            };

            Query_Func.GetParameters(parms, out skip, out take, out seed, out universal, out filters, out includeFacets, out includeTotalCount);
            Assert.AreEqual(5, skip);
            Assert.AreEqual(10, take);
            Assert.AreEqual(null, universal);
            Assert.AreEqual("15", seed);
            Assert.AreEqual(2, filters.Count);
            Assert.AreEqual(true, includeFacets);
            Assert.AreEqual(true, includeTotalCount);

        }
    }
}
