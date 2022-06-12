using System.Threading.Tasks;
using Gyldendal.Jini.Services.Contracts;

namespace Gyldendal.Jini.Services.Core.GpmSubscription
{
   public interface IGpmSubscriptionService
    {
        Task<GpmSubscriptionResponse> Create(string gpmEnvironment, string jiniEnvironment, string subscriptionName);
    }
}
