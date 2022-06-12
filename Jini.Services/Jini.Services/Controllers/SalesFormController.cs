using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using SalesForm = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.SalesForm;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Sales Form Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SalesFormController : ApiController
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
        public SalesFormController(ILogger logger, ILookUpsDbController lookUpsDbController)
        {
            _logger = logger;
            _lookUpsDbController = lookUpsDbController;
        }

        /// <summary>
        /// Get a list of Sales Forms
        /// </summary>
        /// <returns>returns a list of SaleForm</returns>
        [Route("api/v1/SalesForm/GetRefSalesForms")]
        public IHttpActionResult GetRefSalesForms()
        {
            try
            {
                var salesForms = _lookUpsDbController.GetRefSalesForms();
                var result = salesForms.Select(sf => new SalesForm
                {
                    DisplayName = sf.DisplayName,
                    PeriodTypeName = sf.PeriodTypeName,
                    Code = (EnumLicenseType)sf.Code
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