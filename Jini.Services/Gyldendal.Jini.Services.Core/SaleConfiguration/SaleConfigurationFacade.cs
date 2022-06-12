using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Core.SaleConfiguration.Reader;

namespace Gyldendal.Jini.Services.Core.SaleConfiguration
{
    public class SaleConfigurationFacade : ISaleConfigurationFacade
    {
        private ISaleConfigurationReader _saleConfigurationReader;

        public SaleConfigurationFacade(ISaleConfigurationReader saleConfigurationReader)
        {
            _saleConfigurationReader = saleConfigurationReader;
        }

        public SalesConfiguration GetSaleConfiguration(string isbn, WebShop webShop, bool approved)
        {
            return _saleConfigurationReader.GetSaleConfiguration(isbn, webShop, approved);
        }

        public SalesConfiguration GetSaleConfiguration(string isbn, string sellerName, bool approved,string version)
        {
            return _saleConfigurationReader.GetSaleConfigurationForSeller(isbn, sellerName, approved, version);
        }

        public SalesConfiguration GetSaleConfiguration(string isbn, WebShop webShop, bool approved,
            SalesConfigurationType salesConfigType)
        {
            return _saleConfigurationReader.GetSaleConfiguration(isbn, webShop, approved, salesConfigType);
        }
    }
}