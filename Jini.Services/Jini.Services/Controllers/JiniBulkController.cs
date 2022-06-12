using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Core.SaleConfiguration;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Jini Bulk Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class JiniBulkController : ApiController
    {
        private readonly IDeflatedSalesConfigurationFacade _deflatedSalesConfigurationFacade;
        public static int counter { get; set; }
        private readonly ILogger _logger;

        public JiniBulkController(IDeflatedSalesConfigurationFacade deflatedSalesConfigurationFacade, ILogger logger)
        {
            _deflatedSalesConfigurationFacade = deflatedSalesConfigurationFacade;
            _logger = logger;
        }

        /// <summary>
        /// Get deflated sales configuration by filters
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v1/JiniBulk/GetDeflatedSalesConfigurations")]
        [HttpPost]
        public async Task<IHttpActionResult> GetDeflatedSalesConfigurations(DeflatedSalesConfigurationRequest request)
        {
            try
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => request));

                return Ok(await _deflatedSalesConfigurationFacade.GetDeflatedSalesConfiguration(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return InternalServerError(ex);
            }
        }

        ///<summary>
        ///Save Deflated Save Prices in bulk
        ///</summary>
        ///<returns>returns true/false</returns>
        [Route("api/v1/JiniBulk/SaveDeflatedPrices")]
        [HttpPost]
        public IHttpActionResult SaveDeflatedPrices(PriceModelRequest request)
        {
            try
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => request));
                var response = _deflatedSalesConfigurationFacade.SaveDeflatedPrices(request);
                return Ok(new SalesConfigurationResponse { Isbn = request.Isbn, IsUpdated = response, RowId = request.DeflatedPrice[0].Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Ok(false);
            }
        }



    }
}
