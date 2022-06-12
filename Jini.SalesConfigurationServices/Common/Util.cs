using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace Gyldendal.Jini.SalesConfigurationServices.Common
{
    public static class Util
    {
        private const string JsonMediaType = "application/json";
        private static HttpClient CreateClient(string baseAddress, object header = null)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(JsonMediaType));

            //var EkeyKey = ConfigurationManager.AppSettings["EKEY-API-KEY"];
            client.DefaultRequestHeaders.Add("EKEY-API-KEY", "f09ae499-8628-4655-9a5b-aaa3c1e35735");
            client.Timeout = new TimeSpan(0, 3, 0);

            #region Header block
            if (header != null)
            {
                client.DefaultRequestHeaders.Add(header.GetType().GetProperty("Key").GetValue(header, null).ToString(), header.GetType().GetProperty("Value").GetValue(header, null).ToString());

            }
            #endregion

            return client;
        }

        private static async Task<HttpResponseMessage> GetAsync(HttpClient client, string uri)
        {
            var response = await client.GetAsync(uri)
                 .ConfigureAwait(false); // non-blocking call - avoid deadliock using ConfigureAwait false;

            //will throw an exception if not successful
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            var msg = response.Content.ReadAsStringAsync().Result;
            throw new Exception(msg);
        }

        public static T GetAsync<T>(string serviceBaseAddress, string addressSuffix)
        {
            if (string.IsNullOrEmpty(serviceBaseAddress))
                throw new Exception("Base Url not defined");

            using (var client = CreateClient(serviceBaseAddress))
            {
                var response = GetAsync(client, addressSuffix).Result;
                if (response.IsSuccessStatusCode)
                {
                   return response.Content.ReadAsAsync<T>().Result;
                }
                var error = new HttpError { Message = response.ReasonPhrase };
                var jsonString = JsonConvert.SerializeObject(error);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
        }
        
    }
}
