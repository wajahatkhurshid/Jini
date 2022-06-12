using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Core.Product.Services;
using Gyldendal.Jini.Services.Utils;

namespace Gyldendal.Jini.Services.Controllers.V2
{
    /// <summary>
    /// Jini Bulk Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class JiniBulkV2Controller : ApiController
    {
        private readonly ILogger _logger;
        private readonly IProductService _productService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="productService"></param>
        public JiniBulkV2Controller(ILogger logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        /// <summary>
        /// Get deflated sales configuration by filters
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v2/JiniBulkV2/GetDeflatedSalesConfigurations")]
        [HttpPost]
        public async Task<IHttpActionResult> GetDeflatedSalesConfigurations(DeflatedSalesConfigurationRequest request)
        {
            try
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => request), isGdprSafe: true);

                return Ok(await _productService.GetSalesConfigurations(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return InternalServerError(ex);
            }
        }
    }
}
