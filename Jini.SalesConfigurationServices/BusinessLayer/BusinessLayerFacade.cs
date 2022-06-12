using Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Controllers;
using Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Interfaces;
using Gyldendal.Jini.SalesConfigurationServices.Models;
using Gyldendal.Jini.SalesConfigurationServices.Models.Request;

namespace Gyldendal.Jini.SalesConfigurationServices.BusinessLayer
{
    public class BusinessLayerFacade
    {

        #region Constructor

        public BusinessLayerFacade()
        {
            _priceCalculator = new PriceCalculator();
            _salesConfiguration = new SalesConfigurationController();
        }
        #endregion

        #region SalesConfiguration
        public bool IsPublished(string isbn, string jiniServiceUrl, string seller)
        {
            return _salesConfiguration.IsPublished(isbn, jiniServiceUrl, seller);
        }
        public SalesConfiguration GetConfiguration(string isbn, string institutionNumber, string jiniServiceUrl, string uniCServiceUrl, string seller)
        {
            return _salesConfiguration.GetConfiguration(isbn, institutionNumber, jiniServiceUrl, uniCServiceUrl, seller);
        }
        #endregion

        #region Price Calculation
        public PriceRequest GetPrice(string isbn, string institutionNumber, string uniCServiceUrl, PriceRequest salesConfiguration = null)
        {
            return _priceCalculator.GetPrice(isbn, institutionNumber, uniCServiceUrl, salesConfiguration);
        }
        #endregion

        #region Private Variables
        private readonly IPriceCalculator _priceCalculator;
        private readonly ISalesConfiguration _salesConfiguration;
        #endregion
    }
}
