using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Jini.Repository.Contracts;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Data;

namespace Gyldendal.Jini.Repository
{
    public class GpmSubscriptionRepository : BaseRepository<GpmSubscription>, IGpmSubscriptionRepository
    {
        public GpmSubscriptionRepository(Jini_Entities context) : base(context)
        {

        }

        public async Task<List<Subscription>> GetSubscriptionsAsync()
        {
            var subscriptions = await GetAllAsync();
            return subscriptions.Select(x => new Subscription
            {
                SubscripitionName = x.SubscripitionName,
                SubscriptionId = x.SubscriptionId
            }).ToList();

        }
    }
}
