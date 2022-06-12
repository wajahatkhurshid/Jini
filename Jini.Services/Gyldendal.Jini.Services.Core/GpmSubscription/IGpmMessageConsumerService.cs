using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Server;

namespace Gyldendal.Jini.Services.Core.GpmSubscription
{
    public interface IGpmMessageConsumerService
    {
        Task ConsumeGpmEvents(PerformContext context);
    }
}
