using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Gyldendal.AccessServices.Contracts.LicenseModels;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Core.TrialLicense;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Controller for trail license
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class TrailLicenseController : ApiController
    {
        private readonly ITrialLicenseFacade _trialLicenseFacade;

        /// <summary>
        /// Constructor of Trail license
        /// facade will be pushed from dependency containor
        /// </summary>
        /// <param name="trialLicenseFacade"></param>
        public TrailLicenseController(ITrialLicenseFacade trialLicenseFacade)
        {
            _trialLicenseFacade = trialLicenseFacade;
        }

        /// <summary>
        /// Get Link to Share Trial License on WebShops
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(string))]
        [Route("api/v1/Trial/ShareLink/Isbn/{Isbn}/Seller/{seller}")]
        public IHttpActionResult GetTrialLicenseShareLink(string isbn, string seller)
        {
            return Ok(_trialLicenseFacade.GetTrialLicenseShareLink(isbn, seller));
        }

        /// <summary>
        /// Get list of ref trial access forms
        /// </summary>
        /// <returns>list of ref trial access form</returns>
        [HttpGet]
        [ResponseType(typeof(List<TrialAccessForm>))]
        [Route("api/v1/Trial/RefAccessForms/")]
        public IHttpActionResult GetRefTrialAccessForm()
        {
            return Ok(_trialLicenseFacade.GetRefTrialAccessForms());
        }

        /// <summary>
        /// Get list of ref trial period unit types
        /// </summary>
        /// <returns>list of ref trial period unit types</returns>
        [HttpGet]
        [ResponseType(typeof(List<TrialPeriodUnitType>))]
        [Route("api/v1/Trial/RefPeriodUnitTypes/")]
        public IHttpActionResult GetRefTrialPeriodUnitTypes()
        {
            return Ok(_trialLicenseFacade.GetRefTrialPeriodUnitTypes());
        }

        /// <summary>
        /// Get list of ref trial count unit types
        /// </summary>
        /// <returns>list of ref trial count unit types</returns>
        [HttpGet]
        [ResponseType(typeof(List<TrialCountUnitType>))]
        [Route("api/v1/Trial/RefCountUnitTypes/")]
        public IHttpActionResult GetRefTrialCountUnitTypes()
        {
            return Ok(_trialLicenseFacade.GetRefTrialCountUnitTypes());
        }

    }
}