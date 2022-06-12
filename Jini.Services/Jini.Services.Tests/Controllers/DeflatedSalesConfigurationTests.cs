using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Core.Manager;
using Gyldendal.Jini.Services.Core.SaleConfiguration;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Tests.Controllers
{
    [TestClass]
    public class DeflatedSalesConfigurationTests
    {
        private IDeflatedSalesConfigurationFacade _deflatedSalesConfigurationFacade;

        [TestInitialize]
        public void Initialize()
        {
            var deflatedDbController = new Mock<IDeflatedSalesConfigurationDbController>();
            var saleDbController = new Mock<ISaleConfigurationDbController>();

            var rapManager = new Mock<IRapManager>();
            var jiniManager = new Mock<IJiniManager>();
            var logger = new Mock<ILogger>();
            deflatedDbController
                .Setup(x => x.GetDeflatedSalesConfiguration(It.IsAny<DeflatedSalesConfigurationRequest>(), It.IsAny<List<string>>()))
                .ReturnsAsync((ExpectedDeflatedSalesConfigurationDto()));

            _deflatedSalesConfigurationFacade = new DeflatedSalesConfigurationFacade(deflatedDbController.Object,
                rapManager.Object, saleDbController.Object, jiniManager.Object, logger.Object);
        }

        [TestMethod]
        public async Task GetDeflatedSalesConfiguration()
        {
            var result = await _deflatedSalesConfigurationFacade.GetDeflatedSalesConfiguration(
                    new DeflatedSalesConfigurationRequest());

            Assert.IsTrue(result != null);
        }

        private DeflatedSalesConfigurationResponse ExpectedDeflatedSalesConfigurationDto()
        {
            return new DeflatedSalesConfigurationResponse();
        }
    }
}
