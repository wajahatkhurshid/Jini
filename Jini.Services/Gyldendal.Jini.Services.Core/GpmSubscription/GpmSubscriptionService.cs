using Gyldendal.Jini.Services.Contracts;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.ServiceBus.Administration;
using Gyldendal.Jini.Repository.Contracts;

namespace Gyldendal.Jini.Services.Core.GpmSubscription
{
    public class GpmSubscriptionService : IGpmSubscriptionService
    {
        private readonly IGpmSubscriptionRepository _gpmSubscriptionRepository;
        public GpmSubscriptionService(IGpmSubscriptionRepository gpmSubscriptionRepository)
        {
            _gpmSubscriptionRepository = gpmSubscriptionRepository;
        }
        public async Task<GpmSubscriptionResponse> Create(string gpmEnvironment, string jiniEnvironment, string subscriptionName)
        {
            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                subscriptionName = $"{System.Configuration.ConfigurationManager.AppSettings["SubscriptionName"]}- {jiniEnvironment}";
            }
            var gpmBaseUrl = GetGpmBaseUrl();
            var response = new GpmSubscriptionResponse();
            var subscriptionResult = await CreateSubscriber(gpmBaseUrl, subscriptionName);
            response.SubscriptionResult = subscriptionResult;
            var deserializedResultContent = JsonConvert.DeserializeObject<GpmResponse>(subscriptionResult);
            var subscriptionId = deserializedResultContent.id;
            if (subscriptionId <= 0)
            {
                return response;
            }

            var productScopeResult = await CreateSubscriberScopes(subscriptionId, gpmBaseUrl);
            response.ProductScopeResult = productScopeResult;

            await CreateAzureServiceBusSubscription(System.Configuration.ConfigurationManager.AppSettings["TopicName"],
                System.Configuration.ConfigurationManager.AppSettings["SubscriptionName"],
                subscriptionId, System.Configuration.ConfigurationManager.AppSettings["ServiceBusConnectionString"]);
            await PersistSubscriber(subscriptionName, subscriptionId);

            return response;
        }
        private async Task<Response<SubscriptionProperties>> CreateAzureServiceBusSubscription(string topicName, string subscriptionName,
            int correlationFilterSubscriptionId,
            string connectionString)
        {
            var client = new ServiceBusAdministrationClient(connectionString);

            if (await client.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                await client.DeleteSubscriptionAsync(topicName, subscriptionName);
            }

            var filter = new CorrelationRuleFilter();
            filter.ApplicationProperties.Add("subscription-id", correlationFilterSubscriptionId.ToString());
            var ruleOptions = new CreateRuleOptions("subscription-id-correlation-filter", filter);
            var result = await client.CreateSubscriptionAsync(new CreateSubscriptionOptions(topicName, subscriptionName)
            {
                DefaultMessageTimeToLive = TimeSpan.FromDays(200)
            },
            ruleOptions);

            return result;
        }
        private static string GetGpmBaseUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["GpmUrl"];
        }
        private static async Task<string> CreateSubscriber(string gpmBaseUrl, string subscriptionName)
        {
            var path = Path.GetDirectoryName(
                new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            path = Path.GetFullPath(Path.Combine(path, @"..\Jsons\"));
            var subscriptionRequestJson = File.ReadAllText($"{path}SubscriptionRequest.json");

            dynamic jsonObj = JsonConvert.DeserializeObject(subscriptionRequestJson);
            jsonObj["name"] = subscriptionName;

            var transformedRequest = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

            var httpRequest = GetHttpRequest($"{gpmBaseUrl}/api/Subscription", transformedRequest);
            return await GetResponse(gpmBaseUrl, httpRequest);
        }
        private static HttpRequestMessage GetHttpRequest(string uri, string request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));
            httpRequest.Headers.Add("username", "swagger");
            httpRequest.Headers.Add("role", "SuperAdmin");
            httpRequest.Content = new StringContent(request, Encoding.UTF8, "application/json");

            return httpRequest;
        }

        private static async Task<string> GetResponse(string uri, HttpRequestMessage request)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(uri)
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", System.Configuration.ConfigurationManager.AppSettings["GpmApiKey"]);
            var result = await httpClient.SendAsync(request);
            var resultContent = await result.Content.ReadAsStringAsync();
            return resultContent;
        }
        private static async Task<string> CreateSubscriberScopes(int subscriptionId, string baseUri)
        {
            var path = Path.GetDirectoryName(
                new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            path = Path.GetFullPath(Path.Combine(path, @"..\Jsons\"));
            var productScopeResult = await CreateGpmScope($"{path}ProductScopeRequest.json", subscriptionId, baseUri);
            return productScopeResult;
        }
        private static async Task<string> CreateGpmScope(string jsonFileWithPath, int subscriptionId, string gpmBaseUri)
        {
            var request = GetTransformedRequest(jsonFileWithPath, subscriptionId);
            var httpRequest = GetHttpRequest($"{gpmBaseUri}/api/Subscription/Scope", request);
            return await GetResponse(gpmBaseUri, httpRequest);
        }
        private static string GetTransformedRequest(string jsonFileWithPath, int subscriptionId)
        {
            var scopeRequestJson = File.ReadAllText(jsonFileWithPath);

            dynamic jsonObj = JsonConvert.DeserializeObject(scopeRequestJson);
            jsonObj["parentSubscriptionId"] = subscriptionId;
            return JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        }

        private async Task PersistSubscriber(string subscriptionName, int subscriptionId)
        {
            var subscription = new Data.GpmSubscription
            {
                SubscriptionId = subscriptionId,
                SubscripitionName = subscriptionName,
            };

            await _gpmSubscriptionRepository.UpsertAsync(subscription);
        }
    }
}
