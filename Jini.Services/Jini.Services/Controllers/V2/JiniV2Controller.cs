using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Gyldendal.Jini.Services.Core.Departments;
using Gyldendal.Jini.Services.Core.MediaMaterialType;
using Gyldendal.Jini.Services.Utils;

namespace Gyldendal.Jini.Services.Controllers.V2
{
    /// <summary>
    /// JiniV2 Controller 
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class JiniV2Controller : ApiController
    {
        private readonly ILogger _logger;
        private readonly IMediaMaterialTypeService _mediaMaterialTypeService;
        private readonly IDepartmentsService _departmentsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mediaMaterialTypeService"></param>
        public JiniV2Controller(ILogger logger, IMediaMaterialTypeService mediaMaterialTypeService, IDepartmentsService departmentsService)
        {
            _logger = logger;
            _mediaMaterialTypeService = mediaMaterialTypeService;
            _departmentsService = departmentsService;
        }
        /// <summary>
        ///     Get a hierarchical list of media type and their material types from Database
        /// </summary>
        /// <returns>returns a list of media and material types</returns>
        [Route("api/v2/JiniV2/GetMediaAndMaterialTypes")]
        public async Task<IHttpActionResult> GetMediaAndMaterialTypes()
        {
            try
            {
                //Get MediaAndMaterialeTypes from Database
                var result = await _mediaMaterialTypeService.GetMediaAndMaterialTypesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex, isGdprSafe: true);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Get a hierarchical list of departments and sub departments from AX
        /// </summary>
        /// <returns>returns list of departments and sub departments</returns>
        [Route("api/v2/JiniV2/GetDepartmentsAndEditorial")]
        public async Task<IHttpActionResult> GetDepartmentsAndEditorial()
        {
            try
            {
                var result = await _departmentsService.GetDepartmentsAsync();
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
