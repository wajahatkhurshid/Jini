
using Gyldendal.Jini.Services.Contracts;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Jini.Services.Core.GpmSubscription;



namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class GpmSubscriptionController : ApiController
    {
        private readonly IGpmSubscriptionService _gpmSubscriptionService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpmSubscriptionService"></param>
        public GpmSubscriptionController(IGpmSubscriptionService gpmSubscriptionService)
        {
            _gpmSubscriptionService = gpmSubscriptionService;
        }

        /// <summary>
        /// Create subscription and scopes in GPM
        /// </summary>
        /// <param name="gpmEnvironment">Dev,Test,QA,Production</param>
        /// <param name="jiniEnvironment">Dev,Test,QA,Production</param>
        /// <param name="subscriptionName"></param>
        [HttpPost]
        [ResponseType(typeof(GpmSubscriptionResponse))]
        [Route("api/v1/GpmSubscription/CreateSubscription")]
        public async Task<IHttpActionResult> CreateSubscription(string gpmEnvironment, string jiniEnvironment, string subscriptionName)
        {
            var result = await _gpmSubscriptionService.Create(gpmEnvironment, jiniEnvironment, subscriptionName);
            return Ok(result);
        }

    }
}
