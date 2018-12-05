using Microsoft.Azure.Search.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AzureSearch.Api.Test
{
    [TestClass]
    public class Providers_Test
    {
        [TestMethod]
        public void BuildAzureSearchParameters_Test01()
        {
            List<Filter> filters;
            SearchParameters sp;

            sp = Providers.BuildAzureSearchParameters(10, 20, "cancer", new List<Filter>());
            Assert.AreEqual(7, sp.Facets.Count);
            Assert.AreEqual(null, sp.Filter);
            Assert.AreEqual(true, sp.IncludeTotalResultCount);
            Assert.AreEqual(2, sp.OrderBy.Count);
            Assert.AreEqual(QueryType.Simple, sp.QueryType);
            Assert.AreEqual(6, sp.SearchFields.Count);
            Assert.AreEqual(SearchMode.All, sp.SearchMode);
            Assert.AreEqual(2, sp.Select.Count);
            Assert.AreEqual(10, sp.Skip);
            Assert.AreEqual(20, sp.Top);

            filters = new List<Filter>();
            filters.Add(new Filter
            {
                AzureIndexFieldName = "conditions",
                Values = new List<string>()
                {
                    "cancer"
                }
            });
            filters.Add(new Filter
            {
                AzureIndexFieldName = "isMale",
                Values = new List<string>()
                {
                    "true"
                }
            });
            filters.Add(new Filter
            {
                AzureIndexFieldName = "languages",
                Values = new List<string>()
                {
                    "English",
                    "Spanish"
                }
            });
            sp = Providers.BuildAzureSearchParameters(10, 20, "cancer", filters);
            Assert.AreEqual(7, sp.Facets.Count);
            Assert.AreEqual("(conditions/any(i: i eq 'cancer')) and (isMale eq true) and (languages/any(i: i eq 'English')) and (languages/any(i: i eq 'Spanish'))", sp.Filter);
            Assert.AreEqual(true, sp.IncludeTotalResultCount);
            Assert.AreEqual(2, sp.OrderBy.Count);
            Assert.AreEqual(QueryType.Full, sp.QueryType);
            Assert.AreEqual(6, sp.SearchFields.Count);
            Assert.AreEqual(SearchMode.All, sp.SearchMode);
            Assert.AreEqual(2, sp.Select.Count);
            Assert.AreEqual(10, sp.Skip);
            Assert.AreEqual(20, sp.Top);

            filters.Add(new Filter
            {
                AzureIndexFieldName = "isPrimaryCare",
                Values = new List<string>()
                {
                    "true"
                }
            });
            sp = Providers.BuildAzureSearchParameters(10, 20, "cancer", filters);
            Assert.AreEqual(7, sp.Facets.Count);
            Assert.AreEqual("(conditions/any(i: i eq 'cancer')) and (isMale eq true) and (languages/any(i: i eq 'English')) and (languages/any(i: i eq 'Spanish')) and (isPrimaryCare eq true)", sp.Filter);
            Assert.AreEqual(true, sp.IncludeTotalResultCount);
            Assert.AreEqual(2, sp.OrderBy.Count);
            Assert.AreEqual(QueryType.Full, sp.QueryType);
            Assert.AreEqual(6, sp.SearchFields.Count);
            Assert.AreEqual(SearchMode.All, sp.SearchMode);
            Assert.AreEqual(2, sp.Select.Count);
            Assert.AreEqual(10, sp.Skip);
            Assert.AreEqual(20, sp.Top);
        }
    }
}
