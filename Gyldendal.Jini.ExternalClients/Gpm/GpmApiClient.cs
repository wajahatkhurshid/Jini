using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Gyldendal.Jini.ExternalClients.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Gyldendal.Jini.ExternalClients.Gpm
{
    public class GpmApiClient : IGpmApiClient
    {
        private readonly HttpClient _client;

        public GpmApiClient(GpmConfiguration configuration)
        {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = client.BaseAddress ?? configuration.BaseUri;
            client.DefaultRequestHeaders.Add("Authorization", System.Configuration.ConfigurationManager.AppSettings["GpmApiKey"]);
            client.DefaultRequestHeaders.Add("username", configuration.Username);
            client.DefaultRequestHeaders.Add("role", configuration.Role);
            _client = client;
        }
        public async Task<Taxonomy> GetTaxonomyAsync(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Taxonomy/7");
            var response = await _client.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync();
            var taxonomy = JsonConvert.DeserializeObject<TaxonomyDataOutDto>(body);
            var taxonomyResponse = new Taxonomy();
            taxonomyResponse.Id = taxonomy.TaxonomyId;
            taxonomyResponse.RootNodeIds = taxonomy.RootNodeIds.ToList();
            taxonomyResponse.TaxonomyNodes = taxonomy.Nodes.Select(x => new TaxonomyNode()
            {
                ChildNodeIds = x.ChildrenIds.ToList(),
                Level = x.Level,
                Name = x.Name,
                NodeId = x.NodeId,
                ParentNodeId = x.ParentNodeId
            }).ToList();
            return taxonomyResponse;
        }
        public async Task<string> FetchBusinessObjectPayloadAsync(string subscriptionScopeId, string businessObjectId)
        {
            var response = await _client.GetAsync(
                $"/api/Subscriber/scope/{subscriptionScopeId}/businessObject/{businessObjectId}");

            if (!response.IsSuccessStatusCode) return null;

            var businessObjectPayload = await response.Content.ReadAsStringAsync();
            return businessObjectPayload;
        }
    }
}
