using AzureSearch.Common;
using AzureSearch.Common.Dg;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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

            //We need to be case insensitive with the "key" type values.
            List<KeyValuePair<string, string>> lowerQueryParams = TakeQueryParmsToLower(queryParams);

            HttpResponseMessage response;
            DateTime startDt = DateTime.Now;
            QueryResponse qr;

            //Make sure we received valid parameters.
            List<string> failureReasons;
            if (AreParametersAcceptable(lowerQueryParams, out failureReasons) == false)
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

            int skip;int take; string seed; string universal; List<Filter> filters;
            GetParameters(lowerQueryParams, out skip, out take, out seed, out universal, out filters);

            List<AzureSearchProviderQueryResponse> providers = await Providers.GetProviders(skip, take, universal, filters);

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

        public static List<KeyValuePair<string, string>> TakeQueryParmsToLower(List<KeyValuePair<string, string>> queryParms)
        {
            List<KeyValuePair<string, string>> lowerQueryParams = new List<KeyValuePair<string, string>>();
            for (int qp = 0; qp < queryParms.Count; qp++)
            {
                //If value has a filter name, lower that part too.  Validation of the filtername will happen later on.
                string val = queryParms[qp].Value;
                string fn = val.EmGetTextBeforeDelimiter(':');
                if (string.IsNullOrWhiteSpace(fn) == false)
                {
                    fn = fn.ToLower();
                    val = fn + val.Substring(fn.Length);
                }
                lowerQueryParams.Add(new KeyValuePair<string, string>(queryParms[qp].Key.ToLower(), val));
            }
            return lowerQueryParams;
        }

        public static List<Filter> GetFilters(List<KeyValuePair<string, string>> parameters)
        {
            List<Filter> filters = new List<Filter>();
            foreach (KeyValuePair<string, string> kvpair in parameters.Where(p => p.Key == "filter"))
            {
                string filterName = kvpair.Value.EmGetTextBeforeDelimiter(':');
                FilterMapInfo fmInfo = Providers.filterMapInfoList.Single(f => f.FilterName == filterName);
                int index = filters.FindIndex(f => f.AzureIndexFieldName == fmInfo.AzureIndexFieldName);
                if (index < 0)
                {   //We don't already have this one in the filter list.  Add it and its values.
                    filters.Add(new Filter
                    {
                        AzureIndexFieldName = fmInfo.AzureIndexFieldName,
                        Values = new List<string>()
                        {
                            kvpair.Value.Substring(filterName.Length + 1)
                        }
                    });
                    continue;
                }
                //This filter name is already in the filters list.  Just add the values.
                filters[index].Values.Add(kvpair.Value.Substring(filterName.Length + 1));
            }
            return filters;
        }
        public static void GetParameters(List<KeyValuePair<string, string>> parameters, out int skip, out int take, out string seed, out string universal, out List<Filter> filters)
        {
            //At this time all parameters are valid and all "key" type values are in lower case.
            skip = int.Parse(parameters.Single(p => p.Key.EmCompareIgnoreCase("skip")).Value);
            take = int.Parse(parameters.Single(p => p.Key.EmCompareIgnoreCase("take")).Value);

            //Get seed parm if it is there.
            seed = null;
            for (int k = 0; k < parameters.Count; k++)
            {
                if (parameters[k].Key == "seed")
                {
                    seed = parameters[k].Value;
                    break;
                }
            }

            //Get universal parm if it is there.
            universal = null;
            for (int k = 0; k < parameters.Count; k++)
            {
                if (parameters[k].Key == "universal")
                {
                    universal = parameters[k].Value;
                    break;
                }
            }

            //TODO  Come back here for the radius stuff.

            //Get filter parms
            filters = GetFilters(parameters);
            return;
        }

        public static bool AreParametersAcceptable(List<KeyValuePair<string, string>> queryParams, out List<string> failureReasons)
        {
            //The keys and filter name parts of the value of the query parms (what is there before the colon) are all lower case at this point.
            failureReasons = new List<string>();

            if (queryParams.Count == 0)
            {
                failureReasons.Add("No parameters provided.");
            }
            List<KeyValuePair<string, string>> entries;

            //Ensure you only received recognizable parameters.
            List<string> parameterNames = new List<string>()
            {
                "skip",
                "take",
                "seed",
                "universal",
                "filter",
                "latitude",
                "longitude",
                "radius"
            };
            //TODO still need to deal with lat/lon/radius everywhere.
            foreach(KeyValuePair<string, string> parm in queryParams)
            {
                if (parameterNames.Contains(parm.Key) == false)
                {
                    failureReasons.Add($"'{parm.Key}' is not a known parameter.");
                }
            }

            //Validate the 'skip' parameter.
            entries = queryParams
                .Where(q => q.Key == "skip")
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
                    .Where(q => q.Key == "seed")
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
                .Where(q => q.Key == "take")
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
                .Where(q => q.Key == "filter")
                .ToList();

            //We can have "&filter=agesSeen:babies&filter=agesSeen:youth".  This is an AND for agesSeen.  All good so far.
            //But we cannot have more than one suggestion type filter.  So "&filter=condition:cancer&filter=condition:blood" is not allowed.  
            //This is because these type filters come from the suggestions.
            //Try and get the filter names first.
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
                .Intersect(Providers.filterMapInfoList.Where(fmi => fmi.IsSuggestion == true).Select(fmi => fmi.FilterName))
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
                if (Providers.filterMapInfoList.Count(f => f.FilterName == filterNameRequested) == 0)
                {
                    failureReasons.Add($"'{filterNameRequested}' is an invalid filter name.");
                }
            }

            //Need to have at least one filter or one universal search parameter.
            List<KeyValuePair<string, string>> universalSearches = queryParams
                .Where(q => q.Key == "universal")
                .ToList();
            if (universalSearches.Count > 1)
            {
                failureReasons.Add("Can only specify one 'universal' search parameter.");
            }
            if (filters.Count == 0 && universalSearches.Count == 0)
            {
                failureReasons.Add("Must specify at least one universal search or one filter parameter.");
            }

            //Do not allow multiple entries for the boolean types.
            entries = queryParams.Where(q => q.Key == "filter").ToList();
            foreach(KeyValuePair<string, string> fil in entries)
            {
                string filterName = fil.Value.EmGetTextBeforeDelimiter(':');
                if (string.IsNullOrWhiteSpace(filterName))
                {
                    continue;
                }
                FilterMapInfo fmi = Providers.filterMapInfoList.SingleOrDefault(f => f.FilterName == filterName);
                if (fmi == null)
                {
                    continue;
                }
                if (fmi.AzureIndexFieldType != AzureIndexFieldTypes.boolean)
                {
                    continue;
                }
                //Booleans can only have true/false values.
                string val = fil.Value.Substring(filterName.Length);
                if (!(val.EmCompareIgnoreCase("true") || val.EmCompareIgnoreCase("false")))
                {
                    failureReasons.Add($"'{fmi.FilterName}' has an invalid value, namely '{val}'.");
                }
                //There can only be one of these parameter entries for this type of filter.
                int cnt = 0;
                foreach(KeyValuePair<string, string> k in entries)
                {
                    if (k.Value.EmGetTextBeforeDelimiter(':') == filterName)
                    {
                        cnt++;
                    }
                }
                if (cnt > 1)
                {
                    failureReasons.Add($"Can only specify one '{filterName}' filter parameter.");
                }
            }

            return failureReasons.Count == 0;

        }
    }
}
