using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Core.Product.Services;
using Gyldendal.Jini.Services.Filters;
using Gyldendal.Jini.Services.Utils;
using Kendo.Mvc.UI;
using DataSourceRequestModelBinder = Gyldendal.Jini.Services.Models.DataSourceRequestModelBinder;

namespace Gyldendal.Jini.Services.Controllers.V2
{
    /// <summary>
    /// 
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class ProductV2Controller : ApiController
    {
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        /// <summary>
        /// 
        /// </summary>
        public ProductV2Controller(IProductService productService, ILogger logger)
        {
            _productService = productService;
            _logger = logger;
        }
        /// <summary>
        ///     Get all digital products from Database
        /// </summary>
        /// <returns>returns list of digital products</returns>
        [Route("api/v2/ProductV2/GetProducts")]
        [NullValueFilter]
        public async Task<IHttpActionResult> GetProducts(
            [ModelBinder(typeof(DataSourceRequestModelBinder))] DataSourceRequest request)
        {
            _logger.LogInfo(_logger.GetPropertyNameAndValue(() => request), isGdprSafe: true);
            try
            {
                var sortOrder = string.Empty;
                var sortField = string.Empty;
                if (request.Sorts.Count > 0)
                {
                     sortOrder = request.Sorts[0]?.SortDirection.ToString();
                     sortField = request.Sorts[0]?.Member;
                }
                var retVal = (await _productService.GetProductsAsync(request.Filters.ToFilterDescriptor(), request.Page,
                    request.PageSize>0 ? request.PageSize : 50, sortOrder, sortField));
                var result = new DataSourceResult()
                {
                    Data = retVal.Data,
                    Total = retVal.Total

                };
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
        /// <summary>
        ///     Get Product Details of ISBN
        /// </summary>
        /// <returns>returns Json result of details</returns>
        [Route("api/v2/ProductV2/GetProductDetails/{ISBN}")]
        public async Task<IHttpActionResult> GetProductDetails(string isbn)
        {
            try
            {
                var productDetails = await _productService.GetDigitalProductByIsbnAsync(isbn);
                return Json(productDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return NotFound();
            }
        }


        /// <summary>
        ///     Get Grade Levels of ISBN from RAP
        /// </summary>
        /// <returns>returns Json result of GradeLevels</returns>
        [Route("api/v2/ProductV2/GetGradeLevels/{ISBN}")]
        public async Task<IHttpActionResult> GetGradeLevels(string isbn)
        {
            try
            {
                var gradeLevels = await _productService.GetGradeLevelsByIsbnAsync(isbn);
                return Json(gradeLevels);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return NotFound();
            }
        }
    }
}
