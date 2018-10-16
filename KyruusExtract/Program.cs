using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KyruusExtract
{
    class Program
    {
        static string _token;
        static HttpClient _httpClient = new HttpClient();
        static void Main(string[] args)
        {
            Task task = Task.Run(() => Start());
            task.Wait();
        }

        static async Task Start()
        {
            string shuffeSeed = Guid.NewGuid().ToString();
            int page = 1;
            _token = await GetAuthTokenAsync();
            using (TextWriter tw = new StreamWriter(@"C:\kyruusExtract.json", false))
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
            keyValues.Add(new KeyValuePair<string, string>("client_secret", "need good key here"));

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
