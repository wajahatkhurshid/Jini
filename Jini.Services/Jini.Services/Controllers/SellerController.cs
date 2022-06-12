using System.Web.Http;
using System.Web.Http.Cors;
using Gyldendal.Jini.Services.Core.Seller;
using Gyldendal.Jini.Services.Data;
using Gyldendal.Jini.Services.Data.DataAccess;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Get Seller Related Detail
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SellerController : ApiController
    {
        private readonly ISellerFacade _sellerFacade;

        /// <summary>
        /// Constructor for SellerController
        /// </summary>
        /// <param name="sellerFacade"></param>
        public SellerController(ISellerFacade sellerFacade)
        {
            _sellerFacade = sellerFacade;
        }

        /// <summary>
        /// Get seller Id from DB
        /// </summary>
        /// <param name="sellerName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Seller/Get/{sellerName}")]
        public IHttpActionResult GetSellerId(string sellerName)
        {
            return Ok(_sellerFacade.GetSellerId(sellerName));
        }
    }
}