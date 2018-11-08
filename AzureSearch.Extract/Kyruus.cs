using AzureSearch.Common;
using AzureSearch.Extract.Count;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureSearch.Extract
{
    public class Kyruus
    {
        static string _token;
        static int _currentPage;
        static string _content;

        static HttpClient _httpClient = new HttpClient();
        static async Task<int> GetNumberOfProviders()
        {
            _token = await GetAuthTokenAsync();
            string requestUrl = $"https://api.kyruus.com/pm/v8/banner/providers?per_page=0&page=0&facet=1";
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.Headers.Add("Authorization", $"Bearer {_token}");
            request.RequestUri = new Uri(requestUrl);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            KyruusProviderCountResponse responseCount = JsonConvert.DeserializeObject<KyruusProviderCountResponse>(await response.Content.ReadAsStringAsync());
            return responseCount.total_providers;
        }
        public static async Task ExtractWantedOnly()
        {
            int numberOfProviders = await GetNumberOfProviders();
            int pages = numberOfProviders / 100;
            if (numberOfProviders % 100 > 0)
            {
                pages++;
            }
            string shuffeSeed = Guid.NewGuid().ToString();
            using (TextWriter tw = new StreamWriter(@"C:\Temp\kyruusExtractWantedOnly.json", false))
            {
                tw.Write("[");
                for (_currentPage = 1; _currentPage <= pages; _currentPage++)
                {
                    string requestUrl = $"https://api.kyruus.com/pm/v8/banner/providers?per_page=100&page={_currentPage}&shuffle_seed={shuffeSeed}&facet=1";
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Method = HttpMethod.Get;
                    request.Headers.Add("Authorization", $"Bearer {_token}");
                    request.RequestUri = new Uri(requestUrl);
                    HttpResponseMessage response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode == false)
                    {
                        throw new HttpRequestException($"Page call to Kyruus failed.  {response.ReasonPhrase}");
                    }
                    _content = await response.Content.ReadAsStringAsync();
                    KyruusProviderPageResponse kyruusProviderPageResponse = JsonConvert.DeserializeObject<KyruusProviderPageResponse>(_content);
                    List<RelaxedProvider> kyruusDocs = kyruusProviderPageResponse.providers
                        .Where(k => k.show_in_pmc != "No" && k.locations != null && k.locations.Length != 0)
                        .ToList();
                    if (kyruusDocs.Count == 0)
                    {
                        continue;
                    }
                    Provider doc;
                    for (int d = 0; d < kyruusDocs.Count; d++)
                    {
                        try
                        {
                            doc = kyruusDocs[d];
                        }
                        catch (Exception)
                        {   //Skip he junk entry and continue;
                            //TODO report the entry.
                            continue;
                        }
                        string docText = JsonConvert.SerializeObject(doc);
                        tw.Write(docText);
                        if ((_currentPage == pages && d == kyruusDocs.Count - 1) == false)
                        {   //Not the very last provider.  Add a comma after the provider entry.
                            tw.Write(",");
                        }
                    }
                }
                tw.Write("]");
                tw.Flush();
            }
        }

        public static async Task ExtractAll()
        {
            string shuffeSeed = Guid.NewGuid().ToString();
            int page = 1;
            _token = await GetAuthTokenAsync();
            using (TextWriter tw = new StreamWriter(@"C:\Temp\kyruusExtract.json", false))
            {
                tw.Write("[");
                while (true)
                {
                    string requestUrl = $"https://api.kyruus.com/pm/v8/banner/providers?per_page=100&page={page}&shuffle_seed={shuffeSeed}";
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Method = HttpMethod.Get;
                    request.Headers.Add("Authorization", $"Bearer {_token}");
                    request.RequestUri = new Uri(requestUrl);
                    HttpResponseMessage response = await _httpClient.SendAsync(request);
                    string content = await response.Content.ReadAsStringAsync();
                    dynamic kyruusResponse = JsonConvert.DeserializeObject<dynamic>(content);
                    JObject jobject = (JObject)kyruusResponse;
                    JArray jarray = (JArray)jobject["providers"];
                    if (jarray.Count == 0)
                    {
                        break;
                    }
                    if (page != 1)
                    {   //Add comma to the previously last provider entry.
                        tw.Write(",");
                    }
                    string oneHundred = JsonConvert.SerializeObject(jarray);
                    oneHundred = oneHundred.Substring(1, oneHundred.Length - 2);    //Chop of the array open and closing square brackets
                    tw.Write(oneHundred);
                    page++;
                }
                tw.Write("]");
                tw.Flush();
            }
        }
        private async static Task<string> GetAuthTokenAsync()
        {
            DateTimeOffset start = DateTime.UtcNow;

            string requestUri = "https://api.kyruus.com/oauth2/token";
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            keyValues.Add(new KeyValuePair<string, string>("client_id", "banner_website"));
            keyValues.Add(new KeyValuePair<string, string>("client_secret", "68fca452c41b49999120ef052fe49815"));

            FormUrlEncodedContent formFields = new FormUrlEncodedContent(keyValues);
            string formDataText = await formFields.ReadAsStringAsync();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = new StringContent(formDataText);
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            dynamic authResponse = JsonConvert.DeserializeObject<object>(content);
            return authResponse.access_token;
        }

    }
}
