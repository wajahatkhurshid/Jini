using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Data;

namespace Gyldendal.Jini.Repository.Contracts
{
    public interface IGpmSubscriptionRepository : IRepository<GpmSubscription>
    {
        Task<List<Subscription>> GetSubscriptionsAsync();

    }
}
