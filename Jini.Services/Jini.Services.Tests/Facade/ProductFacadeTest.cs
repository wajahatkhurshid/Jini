//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
//using Gyldendal.Jini.Services.Common.ConfigurationManager;
//using Gyldendal.Jini.Services.Common.Utils;
//using Gyldendal.Jini.Services.Core.Manager;
//using Gyldendal.Jini.Services.Core.Product;
//using Gyldendal.Jini.Services.Core.TrialLicense;
//using Gyldendal.Jini.Services.Utils;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using Enums = Gyldendal.Jini.Services.Common.Enums;
//using SalesConfiguration = Gyldendal.Jini.Services.Data.SalesConfiguration;

//namespace Gyldendal.Jini.Services.Tests.Facade
//{
//    [TestClass]
//    public class ProductFacadeTest
//    {
//        private ProductFacade _sut;
//        private Mock<IRapManager> _rapManager;
//        private Mock<IJiniManager> _jiniManager;
//        private Mock<ILogger> _logger;
//        private Mock<IJiniConfigurationManager> _settings;

//        public ProductFacadeTest(Mock<ITrialLicenseFacade> trailAccess)
//        {
//        }

//        [TestInitialize]
//        public void Initialize()
//        {
//            _rapManager = new Mock<IRapManager>();
//            _jiniManager = new Mock<IJiniManager>();
//            _logger = new Mock<ILogger>();
//            _settings = new Mock<IJiniConfigurationManager>();
//            _sut = new ProductFacade(_rapManager.Object, _jiniManager.Object, _logger.Object, _settings.Object);
//        }

//        [TestMethod]
//        [TestCategory(TestConstants.UnitTest)]
//        public async Task GetProductsShouldLogErrorWhenNoProductsFound()
//        {
//            // Assign
//            const string isbn = "2983727509223";
//            List<DigitalProduct> products = new List<DigitalProduct>(); // Deliberate empty!
//            List<SalesConfiguration> configurations = new List<SalesConfiguration> { new SalesConfiguration { Isbn = isbn } };
//            _rapManager.Setup(rm => rm.GetAllDigitalProducts()).ReturnsAsync(products);
//            _jiniManager.Setup(jm => jm.GetAllConfigurations()).Returns(configurations);

//            // Act
//            List<DigitalProduct> actualProducts = await _sut.GetProducts();

//            // Assert
//            _logger.Verify(
//                log =>
//                    log.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<bool>(), It.IsAny<string>(),
//                        It.IsAny<string>(), It.IsAny<int>()), Times.Once);
//        }

//        [TestMethod]
//        [TestCategory(TestConstants.UnitTest)]
//        public async Task GetProductsShouldReturnProductWithActiveConfigurationWhenStateIsApproved()
//        {
//            // Assign
//            const string isbn = "2983727509223";
//            const string active = "Active";
//            List<DigitalProduct> expectedProducts = new List<DigitalProduct> { new DigitalProduct { Isbn = isbn } };
//            List<SalesConfiguration> configurations = new List<SalesConfiguration>
//            {
//                new SalesConfiguration {Isbn = isbn, State = Enums.ToInt(Enums.EnumState.Approved)}
//            };
//            _rapManager.Setup(rm => rm.GetAllDigitalProducts()).ReturnsAsync(expectedProducts);
//            _jiniManager.Setup(jm => jm.GetAllConfigurations()).Returns(configurations);
//            _settings.Setup(set => set.ActiveConfiguration).Returns(active);

//            // Act
//            List<DigitalProduct> actualProducts = await _sut.GetProducts();

//            // Assert
//            Assert.AreEqual("Active", actualProducts[0].ConfigurationText);
//        }

//        [TestMethod]
//        [TestCategory(TestConstants.UnitTest)]
//        public async Task GetProductsShouldReturnProductWithPåbegyndtConfigurationWhenStateIsDraft()
//        {
//            // Assign
//            const string isbn = "2983727509223";
//            List<DigitalProduct> expectedProducts = new List<DigitalProduct> { new DigitalProduct { Isbn = isbn } };
//            List<SalesConfiguration> configurations = new List<SalesConfiguration>
//            {
//                new SalesConfiguration {Isbn = isbn, State = Enums.ToInt(Enums.EnumState.Draft)}
//            };
//            _rapManager.Setup(rm => rm.GetAllDigitalProducts()).ReturnsAsync(expectedProducts);
//            _jiniManager.Setup(jm => jm.GetAllConfigurations()).Returns(configurations);

//            // Act
//            List<DigitalProduct> actualProducts = await _sut.GetProducts();

//            // Assert
//            Assert.AreEqual("Påbegyndt", actualProducts[0].ConfigurationText);
//        }

//        [TestMethod]
//        [TestCategory(TestConstants.UnitTest)]
//        public async Task GetProductsShouldReturnProductWithInactiveConfigurationWhenStateIsZero()
//        {
//            // Assign
//            const string isbn = "2983727509223";
//            const string active = "Inactive";
//            List<DigitalProduct> expectedProducts = new List<DigitalProduct> { new DigitalProduct { Isbn = isbn } };
//            List<SalesConfiguration> configurations = new List<SalesConfiguration>
//            {
//                new SalesConfiguration {Isbn = isbn, State = 0}
//            };
//            _rapManager.Setup(rm => rm.GetAllDigitalProducts()).ReturnsAsync(expectedProducts);
//            _jiniManager.Setup(jm => jm.GetAllConfigurations()).Returns(configurations);
//            _settings.Setup(set => set.InactiveConfiguration).Returns(active);

//            // Act
//            List<DigitalProduct> actualProducts = await _sut.GetProducts();

//            // Assert
//            Assert.AreEqual("Inactive", actualProducts[0].ConfigurationText);
//        }

//        [TestMethod]
//        [TestCategory(TestConstants.UnitTest)]
//        public async Task GetProductDetailsShouldReturnProductWhenIsbnIsFound()
//        {
//            // Assign
//            const string isbn = "2983727509223";
//            List<DigitalProduct> expectedProducts = new List<DigitalProduct> { new DigitalProduct { Isbn = isbn } };
//            _rapManager.Setup(rm => rm.GetAllDigitalProducts()).ReturnsAsync(expectedProducts);

//            // Act
//            DigitalProduct actualProduct = await _sut.GetProductDetails(isbn);

//            // Assert
//            Assert.AreEqual(expectedProducts[0], actualProduct);
//        }

//        [TestMethod]
//        [TestCategory(TestConstants.UnitTest)]
//        public async Task GetProductDetailsShouldNotReturnProductWhenIsbnIsNotMatching()
//        {
//            // Assign
//            const string isbn = "2983727509223";
//            const string wrongIsbn = "298371211119223";
//            List<DigitalProduct> expectedProducts = new List<DigitalProduct> { new DigitalProduct { Isbn = isbn } };
//            _rapManager.Setup(rm => rm.GetAllDigitalProducts()).ReturnsAsync(expectedProducts);

//            // Act
//            DigitalProduct actualProduct = await _sut.GetProductDetails(wrongIsbn);

//            // Assert
//            Assert.IsNull(actualProduct);
//        }
//    }
//}