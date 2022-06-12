using System.Threading;
using System.Threading.Tasks;
using Gyldendal.Jini.ExternalClients.Gpm;

namespace Gyldendal.Jini.ExternalClients.Interfaces
{
    public interface IGpmApiClient
    {
        Task<Taxonomy> GetTaxonomyAsync(CancellationToken cancellationToken);
        Task<string> FetchBusinessObjectPayloadAsync(string subscriptionScopeId, string businessObjectId);
    }
}
