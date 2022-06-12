using Gyldendal.Jini.Services.Controllers;
using Gyldendal.Jini.Services.Core.Product;
using Kendo.Mvc.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Core.Product.Services;
using Gyldendal.Jini.Services.Utils;

namespace Gyldendal.Jini.Services.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        private ProductController _sut;
        private Mock<IProductFacade> _productFacade;
        private Mock<IProductAccessProviderFacade> _productAccessProviderFacade;
        private Mock<ILogger> _logger;
        private Mock<IProductService> _productService;
        [TestInitialize]
        public void Initialize()
        {
            _productFacade = new Mock<IProductFacade>();
            _productAccessProviderFacade = new Mock<IProductAccessProviderFacade>();
            _logger = new Mock<ILogger>();
            _productService = new Mock<IProductService>();
            _sut = new ProductController(_productFacade.Object, _productAccessProviderFacade.Object, _logger.Object, _productService.Object);
        }

        [TestMethod]
        //[TestCategory(TestConstants.UnitTest)]
        public async Task GetProductsShouldReturnProducts()
        {
            // Assign
            const string isbn = "2983727509223";
            List<DigitalProduct> expectedProducts = new List<DigitalProduct> { new DigitalProduct { Isbn = isbn } };
            _productFacade.Setup(pf => pf.GetProducts()).ReturnsAsync(expectedProducts);

            // Act
            DataSourceRequest dataSourceRequest = new DataSourceRequest();
            IHttpActionResult actionResult = await _sut.GetProducts(dataSourceRequest);
            JsonResult<DataSourceResult> jsonResult = (JsonResult<DataSourceResult>)actionResult;

            // Assert
            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(expectedProducts.Count, jsonResult.Content.Total);
            Assert.AreEqual(expectedProducts[0], jsonResult.Content.Data.OfType<DigitalProduct>().First());
        }

        [TestMethod]
        //[TestCategory(TestConstants.UnitTest)]
        public async Task GetProductsShouldThrowWhenNoProductsAreFound()
        {
            // Assign
            _productFacade.Setup(pf => pf.GetProducts()).ThrowsAsync(new Exception());

            // Act
            DataSourceRequest dataSourceRequest = new DataSourceRequest();
            IHttpActionResult actionResult = await _sut.GetProducts(dataSourceRequest);
            JsonResult<DataSourceResult> jsonResult = (JsonResult<DataSourceResult>)actionResult;

            // Assert
            Assert.IsNull(jsonResult);
        }

        [TestMethod]
        //[TestCategory(TestConstants.UnitTest)]
        public async Task GetProductsShouldReturnProductDetails()
        {
            // Assign
            const string isbn = "2983727509223";
            DigitalProduct expectedProduct = new DigitalProduct { Isbn = isbn };
            _productFacade.Setup(pf => pf.GetProductDetails(isbn)).ReturnsAsync(expectedProduct);

            // Act
            IHttpActionResult actionResult = await _sut.GetProductDetails(isbn);
            JsonResult<DigitalProduct> jsonResult = (JsonResult<DigitalProduct>)actionResult;

            // Assert
            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(expectedProduct, jsonResult.Content);
        }

        [TestMethod]
        //[TestCategory(TestConstants.UnitTest)]
        public async Task GetProductDetailsShouldReturnNotFoundWhenNoProductDetailsAreFound()
        {
            // Assign
            const string isbn = "2983727509223";
            _productFacade.Setup(pf => pf.GetProductDetails(isbn)).ThrowsAsync(new Exception());

            // Act
            IHttpActionResult actionResult = await _sut.GetProductDetails(isbn);

            // Assert
            NotFoundResult notFoundResult = (NotFoundResult)actionResult;
            Assert.IsNotNull(notFoundResult);
        }

        //TODO: Add tests for GetGradeLevels()
    }
}