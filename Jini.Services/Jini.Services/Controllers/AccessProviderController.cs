using Gyldendal.Api.ShopServices.Contracts.License.Access;
using Gyldendal.Jini.Services.Data.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Access form Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class AccessProviderController : ApiController
    {
        private readonly ILookUpsDbController _lookUpsDbController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="lookUpsDbController"></param>
        public AccessProviderController(ILookUpsDbController lookUpsDbController)
        {
            _lookUpsDbController = lookUpsDbController;
        }

        /// <summary>
        /// Returns List of AccessProvider from Jini Database
        /// </summary>
        /// <returns>Returns List of Access Forms in Json</returns>
        [Route("api/v1/AccessProvider/GetRefAccessProviders")]
        [ResponseType(typeof(List<AccessProvider>))]
        public IHttpActionResult GetRefAccessProviders()
        {
            var accessProviders = _lookUpsDbController.GetAccessProviders();

            var result = accessProviders.Select(af => new AccessProvider
            {
                Code = (EnumAccessProvider)af.Code,
                DisplayName = af.DisplayName,
                Identifier = af.Identifier
            }).ToList();
            return Ok(result);
        }

        /// <summary>
        /// Returns List of AccessProvider from Jini Database
        /// </summary>
        /// <returns>Returns List of Access Forms in Json</returns>
        [Route("api/v1/AccessProvider/GetRefAccessProviderById")]
        [ResponseType(typeof(AccessProvider))]
        public IHttpActionResult GetRefAccessProviderById(EnumAccessProvider accessProviderCode)
        {
            var accessProvider = _lookUpsDbController.GetAccessProvider((int)accessProviderCode);

            var result = new AccessProvider
            {
                Code = (EnumAccessProvider)accessProvider.Code,
                DisplayName = accessProvider.DisplayName,
                Identifier = accessProvider.Identifier
            };
            return Ok(result);
        }
    }
}