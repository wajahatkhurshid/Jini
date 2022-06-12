using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using PriceModel = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.PriceModel;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Price Model Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class PriceModelController : ApiController
    {
        /// <summary>
        /// Logger Instance
        /// </summary>
        private readonly ILogger _logger;

        private readonly ILookUpsDbController _lookUpsDbController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="lookUpsDbController"></param>
        public PriceModelController(ILogger logger, ILookUpsDbController lookUpsDbController)
        {
            _logger = logger;
            _lookUpsDbController = lookUpsDbController;
        }

        /// <summary>
        /// Get a list of Price Model of Provided Access Form
        /// </summary>
        /// <returns>returns a list of Price Model</returns>
        [Route("api/v1/PriceModel/GetRefPriceModels/{refAccessFormCode}")]
        public IHttpActionResult GetRefPriceModels(EnumAccessForm refAccessFormCode)
        {
            try
            {
                var priceModels = _lookUpsDbController.GetRefPriceModels((int)refAccessFormCode);
                var result = priceModels.Select(pm => new PriceModel
                {
                    DisplayName = pm.DisplayName,
                    Code = (Enums.EnumPriceModel)pm.Code,
                    RefAccessFormCode = (int)refAccessFormCode,
                    ShowPercentage = pm.ShowPercentage
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
    }
}