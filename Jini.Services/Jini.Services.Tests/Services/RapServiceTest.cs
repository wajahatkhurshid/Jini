using Gyldendal.Jini.Services.Common.Services;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Tests.Services
{
    [TestClass]
    public class RapServiceTest
    {
        private RapService _sut;
        private Mock<IServiceHelper> _serviceHelper;

        [TestInitialize]
        public void Initialize()
        {
            _serviceHelper = new Mock<IServiceHelper>();
            _sut = new RapService(_serviceHelper.Object);
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public async Task GetGradeLevelsShouldReturnGradeLevels()
        {
            // Assign
            const string isbn = "34897897435438597";
            List<GradeLevel> expectedLevels = new List<GradeLevel> { new GradeLevel { GradeLevelId = "42" } };
            _serviceHelper.Setup(sh => sh.GetAsync<List<GradeLevel>>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedLevels);

            // Act
            List<GradeLevel> actualLevels = await _sut.GetGradeLevels(isbn);

            // Assert
            Assert.AreEqual(expectedLevels.Count, actualLevels.Count);
            Assert.AreEqual(expectedLevels[0], actualLevels[0]);
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public async Task GetDigitalProductsShouldReturnDigitalProducts()
        {
            // Assign
            const string isbn = "34897897435438597";
            List<DigitalProduct> expectedProducts = new List<DigitalProduct> { new DigitalProduct { Isbn = isbn } };
            _serviceHelper.Setup(sh => sh.GetAsync<List<DigitalProduct>>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(expectedProducts);

            // Act
            List<DigitalProduct> actualProducts = await _sut.GetDigitalProducts();

            // Assert
            Assert.AreEqual(expectedProducts.Count, actualProducts.Count);
            Assert.AreEqual(expectedProducts[0], actualProducts[0]);
        }

        //TODO: Complete this test class for the following methods:
        // - GetNewDigitalProducts
        // - GetDepartmentsAndEditorials
        // - GetMediaAndMaterialeTypes
    }
}