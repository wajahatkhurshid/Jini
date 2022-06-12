using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Gyldendal.Jini.ExternalClients.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Gyldendal.Jini.ExternalClients.AX
{
    public class AxApiClient : IAxApiClient
    {
        private readonly HttpClient _client;

        public AxApiClient(AxConfigurations configuration)
        {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = client.BaseAddress ?? configuration.BaseUri;
            _client = client;
        }
        public async Task<List<Organization>> GetOrganizationsAsync(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Organisation");
            var response = await _client.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync();
            var organizations = JsonConvert.DeserializeObject<Organizations>(body);

            return organizations.OrganizationList;
        }
    }
}
