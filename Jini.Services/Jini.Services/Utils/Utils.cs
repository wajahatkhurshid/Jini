using System;
using System.Data.Entity.Core.EntityClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Gyldendal.Jini.Services.Properties;
using Newtonsoft.Json;

namespace Gyldendal.Jini.Services.Utils
{
    public static class Utils
    {
        private const string JsonMediaType = "application/json";

        public static string RapMetaServiceUrl => Settings.Default.RapMetaServiceUrl;

        public static string JiniConnectionString
        {
            get
            {
                var csb = new EntityConnectionStringBuilder
                {
                    Metadata = "res://*/JiniModel.csdl|res://*/JiniModel.ssdl|res://*/JiniModel.msl",
                    Provider = "System.Data.SqlClient",
                    ProviderConnectionString = BuldProviderConnectionString()
                };
                var entityConnStr = csb.ToString();
                return entityConnStr;
            }
        }

        private static string BuldProviderConnectionString()
        {
            string connStr = $"data source={Settings.Default.DbServer};initial catalog={Settings.Default.DbName};persist security info=True;";
            if (!string.IsNullOrWhiteSpace(Settings.Default.DbUser) && !string.IsNullOrWhiteSpace(Settings.Default.DbPass))
            {
                connStr +=
                    $"user id={Settings.Default.DbUser};password={Settings.Default.DbPass};";
            }
            else
            {
                connStr +=
                    "integrated security=SSPI;";
            }
            connStr += "MultipleActiveResultSets=True;App=EntityFramework";
            return connStr;
        }

        private static HttpClient CreateClient(string baseAddress, object header = null)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(JsonMediaType));
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

        public static T GetAsync<T>(string serviceBaseAddress, params string[] methodAddress)
        {
            if (string.IsNullOrEmpty(serviceBaseAddress))
                throw new Exception("Base Url not defined");
            if (methodAddress == null)
                throw new Exception("Method address not defined");

            var addressSuffix = methodAddress[0];
            for (var i = 1; i < methodAddress.Length; i++)
            {
                addressSuffix += "/" + methodAddress[i];
            }

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

        #region Constants
        public const string RapProductsCacheToken = "RapCache_Products";
        public const string RapAfdelingCacheToken = "RapCache_Afdelings";
        public const string RapMaterialCacheToken = "RapCache_Material";
        public const string RapLastSyncCacheToken = "RapCache_LastSync";
        #endregion

    }
}