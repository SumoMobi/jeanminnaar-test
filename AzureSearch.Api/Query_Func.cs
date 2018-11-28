using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using AzureSearch.Common;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Collections;

namespace AzureSearch.Api
{
    public class QueryResponse
    {
        public string status { get; set; }
        public List<KeyValuePair<string, string>> parameters { get; set; }
        public List<string> failureReasons { get; set; }

        public List<ProviderNarrow> providers { get; set; }

    }
    public static class Query_Func
    {
        [FunctionName("Query")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/query")]HttpRequestMessage req,
            ExecutionContext executionContext,
            TraceWriter log)
        {
            List<KeyValuePair<string, string>> queryParams = req.GetQueryNameValuePairs().ToList();
            List<string> failureReasons;
            HttpResponseMessage response;
            DateTime startDt = DateTime.Now;
            QueryResponse qr;

            if (AreParametersAcceptable(queryParams, out failureReasons) == false)
            {
                failureReasons.Add($"Request received: {req.RequestUri.ToString()}");
                qr = new QueryResponse
                {
                    failureReasons = failureReasons,
                    parameters = queryParams,
                    providers = new List<ProviderNarrow>(),
                    status = "BadRequest"
                };
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonConvert.SerializeObject(qr), System.Text.Encoding.UTF8, "application/json")
                };

                return response;
            }

            qr = new QueryResponse
            {
                failureReasons = failureReasons,
                parameters = queryParams,
                providers = new List<ProviderNarrow>(),
                status = "OK"
            };
            response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(qr), System.Text.Encoding.UTF8, "application/json")
            };

            response.Headers.Add("bh-dg-elapsed-time", (DateTime.Now - startDt).TotalMilliseconds.ToString());
            return response;

        }

        public static async Task<List<ProviderNarrow>> GetProviders(List<KeyValuePair<string, string>> parameters)
        {
            int skip = int.Parse(parameters.Single(p => p.Key.EmCompareIgnoreCase("skip")).Value);
            int take = int.Parse(parameters.Single(p => p.Key.EmCompareIgnoreCase("take")).Value);

            KeyValuePair<string, string>? kvp;

            //Get seed parm if it is there.
            kvp = parameters.SingleOrDefault(p => p.Key.EmCompareIgnoreCase("seed"));
            string seed = null;
            if (kvp != null)
            {
                seed = ((KeyValuePair<string, string>)kvp).Value;
            }

            //Get universal parm if it is there.
            kvp = parameters.SingleOrDefault(p => p.Key.EmCompareIgnoreCase("universal"));
            string universal = null;
            if (kvp != null)
            {
                universal = ((KeyValuePair<string, string>)kvp).Value;
            }

            //Get filter parms
            List<Filter> filters = new List<Filter>();
            foreach (KeyValuePair<string, string> kvpair in parameters)
            {
                if (kvpair.Key.EmCompareIgnoreCase("filter") == false)
                {
                    continue;
                }
                string filterName = kvpair.Value.EmGetTextBeforeDelimiter(':');
                int index = filters.FindIndex(f => f.FilterName.EmCompareIgnoreCase(filterName));
                if (index < 0)
                {
                    filters.Add(new Filter
                    {
                        FilterName = filterName,
                        Values = new List<string>()
                        {
                            kvpair.Value.Substring(filterName.Length)
                        }
                    });
                    continue;
                }
                filters[index].Values.Add(kvpair.Value);
            }
            List<AzureSearchProviderQueryResponse> providers = await Providers.GetProviders(skip, take, universal, filters);
//            providers = Randomize(providers, seed);
            return providers;
        }

        public static bool AreParametersAcceptable(List<KeyValuePair<string, string>> queryParams, out List<string> failureReasons)
        {
            failureReasons = new List<string>();

            if (queryParams.Count == 0)
            {
                failureReasons.Add("No parameters provided.");
            }
            List<KeyValuePair<string, string>> entries;

            //Ensure you only received recognizable parameters.
            List<string> parameterNames = new List<string>()
            {
                "skip","" +
                "take",
                "seed",
                "universal",
                "filter",
                "latitude",
                "longitude",
                "radius"
            };
            foreach(KeyValuePair<string, string> parm in queryParams)
            {
                if (parameterNames.Contains(parm.Key, new EmStringEqualityComparerCaseInsensitive()) == false)
                {
                    failureReasons.Add($"'{parm.Key}' is not a known parameter.");
                }
            }

            //Validate the 'skip' parameter.
            entries = queryParams
                .Where(q => q.Key.EmCompareIgnoreCase("skip"))
                .ToList();
            if (entries.Count == 0)
            {
                failureReasons.Add("'skip' parameter is required.");
            }
            if (entries.Count > 1)
            {
                failureReasons.Add("'skip' parameter provided more than once.");
            }
            if (entries.Count == 1)
            {
                int skip;
                if (int.TryParse(entries[0].Value, out skip) == false)
                {
                    failureReasons.Add($"The 'skip' parameter value must be a number.  Received {entries[0].Value}.");
                }
                //Seed must be provided on any subsequent pages.  It should in some cases be required on the first page too when the user navigates back to the first page.  But that we don't know here.
                entries = queryParams
                    .Where(q => q.Key.EmCompareIgnoreCase("seed"))
                    .ToList();
                if (entries.Count > 1)
                {
                    failureReasons.Add("There can only be one 'seed' parameter.");
                }
                if (skip > 0 && entries.Count == 0)
                {
                    failureReasons.Add("'seed' required on subsequent page calls.");
                }
            }

            //Validate the 'take' parameter.
            entries = queryParams
                .Where(q => q.Key.EmCompareIgnoreCase("take"))
                .ToList();
            if (entries.Count == 0)
            {
                failureReasons.Add("'take' parameter is required.");
            }
            if (entries.Count > 1)
            {
                failureReasons.Add("'take' parameter provided more than once.");
            }
            if (entries.Count == 1)
            {
                int take;
                if (int.TryParse(entries[0].Value, out take) == false)
                {
                    failureReasons.Add($"The 'take' parameter value must be a number.  Received {entries[0].Value}.");
                }
                else
                {
                    if (take <= 0)
                    {
                        failureReasons.Add("'take' has to be greater than 0.");
                    }
                    if (take > 100)
                    {
                        failureReasons.Add("'take' cannot exceed 100.");
                    }
                }
            }

            //Validate 'filter' parameters
            List<KeyValuePair<string, string>> filters = queryParams
                .Where(q => q.Key.EmCompareIgnoreCase("filter"))
                .ToList();
            List<string> suggestionFilterNames = new List<string>()
            {
                "Condition",
                "Provider", //Name search
                "Specialty",
                "Insurance",
                "PrimaryCare",  //Used when user selects the fake PC entry.
            };
            List<string> filterNames = new List<string>()
            {
//                "Condition_Specialist",
//                "Condition_PrimaryCare",
                "Condition",
                "Provider", //Name search
                "Specialty",
                "Insurance",
                "PrimaryCare",  //When used?  When user selects the fake PC entry.
                "agesSeen",
                "acceptedInsurances",
                "acceptNewPatients",
                "isMale",
                "providerType",
                "languages",    //Provide for multiple selection (which are then ORs and not ANDs)
                "networkAffiliations"
            };
            //We can have "&filter=agesSeen:babies&filter=agesSeen:youth".  This is an OR for agesSeen.  All good so far.
            //But we cannot have more than one facet.  So "&filter=condition:cancer&filter=condition:blood" is not allowed.  This is because the facets come from the suggestions.
            //Try and get the facet names first.
            List<string> filterNamesRequested = filters
                .Select(f => f.Value.EmGetTextBeforeDelimiter(':'))
                .ToList();
            List<string> formattedFilterNamesRequested = filterNamesRequested
                .Where(f => f != null && f.Length != 0)
                .ToList();
            if (formattedFilterNamesRequested.Count != filterNamesRequested.Count)
            {
                failureReasons.Add("Received one or more filters without a facet name, colon or value.");
            }

            List<string> duplicateSuggestions = formattedFilterNamesRequested
                .Select(q => q.ToLower())
                .Intersect(suggestionFilterNames
                .Select(f => f.ToLower()))
                .ToList();
            foreach(string dupSuggestion in duplicateSuggestions)
            { 
                failureReasons.Add($"Received more than one suggestion parameter, namely {dupSuggestion}.");
            }
            //Check formatting of filter parameters.
            foreach (KeyValuePair<string, string> filter in filters)
            {
                string filterName = filter.Value.EmGetTextBeforeDelimiter(':');
                if (filterName == null)
                {
                    failureReasons.Add($"Invalid filter value, {filter.Value}.  Missing the colon.");
                }
                else
                {
                    if (filterName.Length == 0)
                    {
                        failureReasons.Add($"Invalid filter value, {filter.Value}.  Search value not provided.");
                    }
                }
            }

            //Check for valid filter names.
            foreach (string filterNameRequested in formattedFilterNamesRequested)
            {
                if (filterNames.Contains(filterNameRequested, new EmStringEqualityComparerCaseInsensitive()) == false)
                {
                    failureReasons.Add($"'{filterNameRequested}' is an invalid filter name.");
                }
            }

            //Need to have at least one filter or one universal search parameter.
            List<KeyValuePair<string, string>> universalSearches = queryParams
                .Where(q => q.Key.EmCompareIgnoreCase("universal"))
                .ToList();
            if (universalSearches.Count > 1)
            {
                failureReasons.Add("Can only specify one 'universal' search parameter.");
            }
            if (filters.Count == 0 && universalSearches.Count == 0)
            {
                failureReasons.Add("Must specify at least one universal search or one filter parameter.");
            }

            return failureReasons.Count == 0;

        }
    }
}
