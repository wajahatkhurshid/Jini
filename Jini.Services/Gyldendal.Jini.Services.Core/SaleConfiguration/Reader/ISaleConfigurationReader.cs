using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;

namespace Gyldendal.Jini.Services.Core.SaleConfiguration.Reader
{
    public interface ISaleConfigurationReader
    {
        SalesConfiguration GetSaleConfigurationForSeller(string isbn, string sellerName, bool approved,string version);

        SalesConfiguration GetSaleConfiguration(string isbn, WebShop webShop, bool approved);

        SalesConfiguration GetSaleConfiguration(string isbn, WebShop webShop, bool approved,
            SalesConfigurationType salesConfigType);
    }
}