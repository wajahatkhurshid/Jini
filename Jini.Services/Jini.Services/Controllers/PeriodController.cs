using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    ///  Period Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class PeriodController : ApiController
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
        public PeriodController(ILogger logger, ILookUpsDbController lookUpsDbController)
        {
            _logger = logger;
            _lookUpsDbController = lookUpsDbController;
        }

        /// <summary>
        /// Get a list of Periods of provided salesForm
        /// </summary>
        /// <returns>returns list of Periods</returns>
        [Route("api/v1/Period/GetRefPeriods/{refSalesFormCode}")]
        public IHttpActionResult GetRefPeriods(EnumLicenseType refSalesFormCode)
        {
            try
            {
                var periods = _lookUpsDbController.GetRefPeriods((int)refSalesFormCode);
                var result = periods.Select(p => new Period
                {
                    //RefSalesFormCode = p.RefSalesForm.Code,
                    RefPeriodUnitTypeCode = (EnumPeriodUnitType)p.RefPeriodUnitType.Code,
                    Id = p.Id,

                    DisplayName = $"{p.UnitValue} {p.RefPeriodUnitType.DisplayName}",
                    UnitValue = p.UnitValue,
                    RefSalesFormCode = refSalesFormCode
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Get a list of Period Types
        /// </summary>
        /// <returns>returns list of Periods</returns>
        [Route("api/v1/Period/GetRefPeriodUnitTypes/")]
        public IHttpActionResult GetRefPeriodUnitTypes()
        {
            try
            {
                var periods = _lookUpsDbController.GetRefPeriodUnitTypes();
                var result = periods.Select(p => new PeriodUnitType()
                {
                    DisplayName =
                        p.DisplayName,
                    Code = (EnumPeriodUnitType)p.Code
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