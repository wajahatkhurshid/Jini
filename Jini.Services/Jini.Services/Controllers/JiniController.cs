using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Core.Manager;
using Gyldendal.Jini.Services.Utils;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Jini Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class JiniController : ApiController
    {
        private readonly IRapManager _rapManager;
        private readonly ILogger _logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rapManager"></param>
        public JiniController(IRapManager rapManager, ILogger logger)
        {
            _rapManager = rapManager;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Jini/GetLastSyncTime")]
        public IHttpActionResult GetLastSyncTime()
        {
            var lastSyncTime = MemoryCacher.GetValue(Common.Utils.Utils.RapLastSyncCacheToken);
            return Json(lastSyncTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Jini/ClearCache")]
        public IHttpActionResult ClearCache()
        {
            MemoryCacher.Delete(Common.Utils.Utils.RapProductsCacheToken);
            MemoryCacher.Delete(Common.Utils.Utils.RapAfdelingCacheToken);
            MemoryCacher.Delete(Common.Utils.Utils.RapMaterialCacheToken);
            return Ok();
        }

        /// <summary>
        ///     Get a hierarchical list of departments and sub departments from RAP
        /// </summary>
        /// <returns>returns list of departments and sub departments</returns>
        [Route("api/v1/Jini/GetDepartmentsAndEditorial")]
        public async Task<IHttpActionResult> GetDepartmentsAndEditorial()
        {
            try
            {
                //Get MediaAndMaterialeTypes from RAP
                var result = await _rapManager.GetDepartmentsAndEditorials();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex, isGdprSafe: true);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Get a hierarchical list of media type and their material types from RAP
        /// </summary>
        /// <returns>returns a list of media and material types</returns>
        [Route("api/v1/Jini/GetMediaAndMaterialeTypes")]
        public async Task<IHttpActionResult> GetMediaAndMaterialeTypes()
        {
            try
            {
                //Get MediaAndMaterialeTypes from RAP
                var result = await _rapManager.GetMediaAndMaterialeTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex, isGdprSafe: true);
                return InternalServerError(ex);
            }
        }

    }
}
