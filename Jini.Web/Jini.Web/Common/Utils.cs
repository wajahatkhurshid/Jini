using System;
using System.DirectoryServices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace Gyldendal.Jini.Web.Common
{
    public static class Utils
    {
        public static string JiniServiceUrl => Properties.Settings.Default.JiniServiceUrl;
        public static string MmsUrl => Properties.Settings.Default.MmsUrl;
        public static string Vat => Properties.Settings.Default.Vat;
        private const string JsonMediaType = "application/json";
        public static string SellerName => Properties.Settings.Default.SellerName;
        public static string GetLoggedInUserName()
        {
            {
                try
                {
                    DirectoryEntry de = new DirectoryEntry("WinNT://" + HttpContext.Current.User.Identity.Name?.Replace("\\", "/"));
                    return de.Properties["fullName"].Value.ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
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
    }
}