
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gyldendal.Jini.ExternalClients.AX;

namespace Gyldendal.Jini.ExternalClients.Interfaces
{
    public interface IAxApiClient
    {
        Task<List<Organization>> GetOrganizationsAsync(CancellationToken cancellationToken);
    }
}
