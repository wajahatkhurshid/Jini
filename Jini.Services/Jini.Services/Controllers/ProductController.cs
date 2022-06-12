using Gyldendal.Api.ShopServices.Contracts.License.Access;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Jini.Services.Common;
using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.ErrorHandling;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Core.Product;
using Gyldendal.Jini.Services.Filters;
using Gyldendal.Jini.Services.Utils;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Gyldendal.Jini.Services.Core.Product.Services;
using DataSourceRequestModelBinder = Gyldendal.Jini.Services.Models.DataSourceRequestModelBinder;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Product Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class ProductController : ApiController
    {
        private readonly IProductFacade _productFacade;
        private readonly IProductAccessProviderFacade _productAccessProviderFacade;
        private readonly ILogger _logger;
        private readonly IProductService _productService;

        /// <summary>
        /// Product Controller Constructor
        /// </summary>
        /// <param name="productFacade"></param>
        /// <param name="productAccessProviderFacade"></param>
        /// <param name="logger"></param>
        /// <param name="productService"></param>
        public ProductController(IProductFacade productFacade, IProductAccessProviderFacade productAccessProviderFacade, ILogger logger, IProductService productService)
        {
            _productFacade = productFacade;
            _productAccessProviderFacade = productAccessProviderFacade;
            _logger = logger;
            _productService = productService;
        }

        /// <summary>
        ///     Get all digital products from RAP
        /// </summary>
        /// <returns>returns list of digital products</returns>
        [Route("api/v1/Product/GetProducts")]
        [NullValueFilter]
        public async Task<IHttpActionResult> GetProducts(
            [ModelBinder(typeof(DataSourceRequestModelBinder))] DataSourceRequest request)
        {
            _logger.LogInfo(_logger.GetPropertyNameAndValue(() => request));
            try
            {
                var retVal = (await _productFacade.GetProducts()).ToDataSourceResult(request ?? new DataSourceRequest());
                return Json(retVal);
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
        [Route("api/v1/Product/GetProductDetails/{ISBN}")]
        public async Task<IHttpActionResult> GetProductDetails(string isbn)
        {
            try
            {
                var productDetails = await _productFacade.GetProductDetails(isbn);
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
        [Route("api/v1/Product/GetGradeLevels/{ISBN}")]
        public async Task<IHttpActionResult> GetGradeLevels(string isbn)
        {
            try
            {
                var gradeLevels = string.Empty;

                var gradeLevelList = await _productFacade.GetGradeLevels(isbn);
                if (gradeLevelList.Any())
                {
                    gradeLevels = ProcessGradeLevels.SortGradeLevels(gradeLevelList);
                    gradeLevels = string.Join(",", gradeLevels);
                }
                return Json(gradeLevels);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return NotFound();
            }
        }

        #region ProductAccessProvider

        /// <summary>
        /// Return the list of product accessProviders of isbns
        /// </summary>
        /// <param name="isbns"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Product/GetProductAccessProviders/")]
        [ResponseType(typeof(List<ProductAccessProvider>))]
        public IHttpActionResult GetProductAccessProviders(List<string> isbns)
        {
            return Ok(_productAccessProviderFacade.GetProductAccessProviders(isbns));
        }

        /// <summary>
        /// Tells either the specified accessProvider exists for isbn or not
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="enumAccessProvider"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Product/IsProductAccessProviderExists/{isbn}/{enumAccessProvider}")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult IsProductAccessProviderExists(string isbn, EnumAccessProvider enumAccessProvider)
        {
            return Ok(_productAccessProviderFacade.IsProductAccessProviderExists(isbn, (int)enumAccessProvider));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        [Route("api/v1/Product/GetAllProductAccessProviders/{isbn}")]
        [ResponseType(typeof(ProductAccessProviders))]
        public IHttpActionResult GetAllProductAccessProviders(string isbn)
        {
            return Ok(_productAccessProviderFacade.GetProductAccessProviders(isbn));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="productAccessProvider"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Product/SaveProductAccessProviders/")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveProductAccessProviders(ProductAccessProviders productAccessProvider)
        {
            if (productAccessProvider == null)
                throw new ProcessException((ulong)ErrorCodes.NullValue, ErrorCodes.NullValue.GetDescription(), Constants.OrigionatingSystemName);

            return Ok(_productAccessProviderFacade.SaveProductAccessProviders(productAccessProvider));
        }

        #endregion ProductAccessProvider

    }
}