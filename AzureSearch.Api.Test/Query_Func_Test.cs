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
    }
}
