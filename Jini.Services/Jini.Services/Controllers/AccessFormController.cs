using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using AccessForm = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.AccessForm;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Access form Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class AccessFormController : ApiController
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
        public AccessFormController(ILogger logger, ILookUpsDbController lookUpsDbController)
        {
            _logger = logger;
            _lookUpsDbController = lookUpsDbController;
        }

        /// <summary>
        /// Returns List of Access Forms from Jini Database
        /// </summary>
        /// <returns>Returns List of Access Forms in Json</returns>
        [Route("api/v1/AccessForm/GetRefAccessForms")]
        public IHttpActionResult GetRefAccessForms()
        {
            try
            {
                var accessForms = _lookUpsDbController.GetRefAccessForms();
                var result = accessForms.Select(af => new AccessForm
                {
                    Code = (EnumAccessForm)af.Code,
                    DisplayName = af.DisplayName
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