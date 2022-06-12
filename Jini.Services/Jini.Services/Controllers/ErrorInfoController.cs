using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Common.WebUtils.Models;
using Gyldendal.Jini.Services.Common.Utils;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// For Exposing Errors
    /// </summary>
    public class ErrorInfoController : ApiController
    {
        private readonly IErrorCodeUtil _errorCodeUtil;

        /// <summary>
        /// Constructor of ErrorInfo Controller for getting the error codes
        /// </summary>
        /// <param name="errorCodeUtil"></param>
        public ErrorInfoController(IErrorCodeUtil errorCodeUtil)
        {
            _errorCodeUtil = errorCodeUtil;
        }

        /// <summary>
        /// Get error details for the specified error code of CoreData
        /// <param name="errorCode"></param>
        /// </summary>
        /// <returns>Error code details</returns>
        [Route("api/v1/Error/Get/Details/{errorCode}")]
        [ResponseType(typeof(ErrorDetail))]
        [HttpGet]
        public IHttpActionResult GetErrorCodeDetails(ulong errorCode)
        {
            return Ok(_errorCodeUtil.GetErrorDetail(errorCode));
        }

        /// <summary>
        /// Get All ErrorCodes of Jini
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/Error/Get")]
        [ResponseType(typeof(IEnumerable<ErrorDetail>))]
        [HttpGet]
        public IHttpActionResult GetAllErrorCodes()
        {
            return Ok(_errorCodeUtil.GetAllErrorCodes());
        }
    }
}