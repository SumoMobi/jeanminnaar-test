using AzureSearch.Common;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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
        #region status info
        public string status { get; set; }
        public List<NameValue> queryParameters { get; set; }
        public List<string> failureReasons { get; set; }
        #endregion

        #region results
        public long? count { get; set; }
        public FacetResults facets { get; set; }
        public List<AzureSearchProviderRequestedFields> documents { get; set; }
        #endregion
    }

    public class NameValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    public static class Query_Func
    {
        [FunctionName("Query")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/azuresearch/query")]HttpRequestMessage req,
            ExecutionContext executionContext,
            ILogger log)
        {
            List<NameValue> queryParams = new List<NameValue>(); 
            string query = req.RequestUri.Query;
            query = WebUtility.UrlDecode(query);
            if (query != null && query.Length > 1)
            {   //More than just the question mark
                string[] parms = query.Substring(1).Split('&');
                foreach(string parm in parms)
                {
                    string[] keyValue = parm.Split('=');
                    switch (keyValue.Length)
                    {
                        case 0:
                            break;
                        case 1: //= not found
                            queryParams.Add(new NameValue(parm, ""));
                            break;
                        case 2:
                            queryParams.Add(new NameValue(keyValue[0], keyValue[1]));
                            break;
                        default:
                            queryParams.Add(new NameValue(keyValue[0], parm.Substring(keyValue[0].Length + 1)));
                            break;
                    }
                }
            }

            //We need to be case insensitive with the "key" type values.
            List<NameValue> lowerQueryParams = TakeQueryParmsToLower(queryParams);

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
                    count = null,
                    documents = null,
                    facets = null,
                    failureReasons = failureReasons,
                    queryParameters = queryParams,
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

            DocumentSearchResult<AzureSearchProviderRequestedFields> results = await Providers.GetProviders(skip, take, universal, filters);

            qr = new QueryResponse
            {
                count = results.Count,
                documents = results.Results.Select(r => r.Document).ToList(),
                facets = results.Facets,
                failureReasons = failureReasons,
                queryParameters = queryParams,
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

        public static List<NameValue> TakeQueryParmsToLower(List<NameValue> queryParms)
        {
            List<NameValue> lowerQueryParams = new List<NameValue>();
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
                lowerQueryParams.Add(new NameValue(queryParms[qp].Name.ToLower(),val));
            }
            return lowerQueryParams;
        }

        public static List<Filter> GetFilters(List<NameValue> parameters)
        {
            List<Filter> filters = new List<Filter>();
            foreach (NameValue nv in parameters.Where(p => p.Name == "filter"))
            {
                string filterName = nv.Value.EmGetTextBeforeDelimiter(':');
                FilterMapInfo fmInfo = Providers.filterMapInfoList.Single(f => f.FilterName == filterName);
                int index = filters.FindIndex(f => f.AzureIndexFieldName == fmInfo.AzureIndexFieldName);
                if (index < 0)
                {   //We don't already have this one in the filter list.  Add it and its values.
                    if (filterName == "condition_primarycare")
                    {   //There can only be one of these filters.
                        filters.Add(new Filter
                        {
                            AzureIndexFieldName = fmInfo.AzureIndexFieldName,
                            Values = new List<string>()
                            {
                                "true"
                            }
                        });
                        continue;
                    }
                    filters.Add(new Filter
                    {
                        AzureIndexFieldName = fmInfo.AzureIndexFieldName,
                        Values = new List<string>()
                        {
                            nv.Value.Substring(filterName.Length + 1)
                        }
                    });
                    continue;
                }
                //This filter name is already in the filters list.  Just add the values.
                filters[index].Values.Add(nv.Value.Substring(filterName.Length + 1));
            }
            return filters;
        }
        public static void GetParameters(List<NameValue> parameters, out int skip, out int take, out string seed, out string universal, out List<Filter> filters)
        {
            //At this time all parameters are valid and all "key" type values are in lower case.
            skip = int.Parse(parameters.Single(p => p.Name.EmCompareIgnoreCase("skip")).Value);
            take = int.Parse(parameters.Single(p => p.Name.EmCompareIgnoreCase("take")).Value);

            //Get seed and universal parms if they are there.
            seed = null;
            universal = null;
            for (int k = 0; k < parameters.Count; k++)
            {
                if (parameters[k].Name == "seed")
                {
                    seed = parameters[k].Value;
                    if (universal != null)
                    {
                        break;
                    }
                }
                if (parameters[k].Name == "universal")
                {
                    universal = parameters[k].Value;
                    if (seed != null)
                    {
                        break;
                    }
                }
            }

            //TODO  Come back here for the radius stuff.

            //Get filter parms
            filters = GetFilters(parameters);
            return;
        }

        public static bool AreParametersAcceptable(List<NameValue> queryParams, out List<string> failureReasons)
        {
            //The keys and filter name parts of the value of the query parms (what is there before the colon) are all lower case at this point.
            failureReasons = new List<string>();

            if (queryParams.Count == 0)
            {
                failureReasons.Add("No parameters provided.");
            }
            List<NameValue> entries;

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
            foreach(NameValue parm in queryParams)
            {
                if (parameterNames.Contains(parm.Name) == false)
                {
                    failureReasons.Add($"'{parm.Name}' is not a known parameter.");
                }
            }

            //Validate the 'skip' parameter.
            entries = queryParams
                .Where(q => q.Name == "skip")
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
                    .Where(q => q.Name == "seed")
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
                .Where(q => q.Name == "take")
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
            List<NameValue> filters = queryParams
                .Where(q => q.Name == "filter")
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

            //List<string> suggestions = formattedFilterNamesRequested
            //    .Intersect(Providers.filterMapInfoList.Where(fmi => fmi.IsSuggestion == true).Select(fmi => fmi.FilterName))
            //    .ToList();
            //if (suggestions.Count > 1)
            //{
            //    foreach (string dupSuggestion in suggestions)
            //    {
            //        failureReasons.Add($"Received more than one suggestion parameter, namely {dupSuggestion}.");
            //    }
            //}

            //Check formatting of filter parameters.
            foreach (NameValue filter in filters)
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
            List<NameValue> universalSearches = queryParams
                .Where(q => q.Name == "universal")
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
            entries = queryParams.Where(q => q.Name == "filter").ToList();
            foreach(NameValue fil in entries)
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
                string val = fil.Value.Substring(filterName.Length + 1);
                if (!(val.EmCompareIgnoreCase("true") || val.EmCompareIgnoreCase("false")))
                {
                    if (filterName != "conditions_primarycare")
                    {   //The search value will later in Providers.BuildParameters be converted to a boolean true value.
                        failureReasons.Add($"'{fmi.FilterName}' has an invalid value, namely '{val}'.");
                    }
                }
                //There can only be one of these parameter entries for this type of filter.
                int cnt = 0;
                foreach(NameValue k in entries)
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
