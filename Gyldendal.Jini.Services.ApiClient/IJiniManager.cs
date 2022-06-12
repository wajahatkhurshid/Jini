using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.License.Access;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.Response;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.ApiClient
{
    public interface IJiniManager
    {
        List<int> IsSalesConfigurationPublished(WebShop webShop, string isbn);

        SalesConfiguration GetProductSaleConfiguration(WebShop webShop, string isbn);

        SalesConfiguration GetProductSaleConfiguration(WebShop webShop, string isbn,
            SalesConfigurationType salesConfigType);

        IEnumerable<UpdatedSaleConfigInfo> GetUpdatedSaleConfigsInfo(long updatedAfter, WebShop webShop, int take);

        /// <summary>
        ///     Get Grade Levels of ISBN from RAP
        /// </summary>
        /// <returns>returns string of GradeLevels</returns>
        string GetGradeLevels(string isbn);

        /// <summary>
        ///  Tells either the specified accessProvider exists for isbn or not
        /// </summary>
        /// <returns></returns>
        bool IsProductAccessProviderExists(string isbn, EnumAccessProvider enumAccessProvider);

        /// <summary>
        ///  Return the list of product accessProviders of isbns
        /// </summary>
        /// <returns></returns>
        List<ProductAccessProvider> GetProductAccessProviders(List<string> isbns);
    }
}