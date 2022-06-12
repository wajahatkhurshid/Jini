using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Enums = Gyldendal.Jini.Services.Common.Enums;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public interface ISaleConfigurationDbController
    {
        SalesConfiguration GetSaleConfiguration(string isbn, Enums.EnumState state, WebShop webShop, SalesConfigurationType salesConfigType);

        /// <summary>
        /// Get Sales Configuration for webshop
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="webShop"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        SalesConfiguration GetSaleConfigurationForWebShop(string isbn, WebShop webShop, Enums.EnumState state);

        /// <summary>
        /// Get Sales configuration Exists or not for Front End
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        List<int> SalesConfigurationExistBySeller(string isbn, string seller , int refSalesConfigTypeCode = (int)SalesConfigurationType.External);

        /// <summary>
        /// Get whether SalesConfiguration exists or not for webshop
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="webShop"></param>
        /// <returns></returns>
        List<int> SalesConfigurationExistByWebShop(string isbn, WebShop webShop);

        List<SalesConfiguration> GetSalesConfigurations(string isbn, Api.ShopServices.Contracts.Enumerations.Seller seller, int refSalesConfigTypeCode= (int)SalesConfigurationType.External);
        SalesConfiguration GetSalesConfigurationByState(string isbn, int stateId, string seller);
        bool CreateSalesConfiguration(SalesConfiguration configuration);

        bool UpdateSalesConfiguration(List<SalesConfiguration> oldConfigurations,
            SalesConfiguration newConfiguration);

        bool UpdateBulkSalesConfiguration(List<SalesConfiguration> oldConfigurations,
           SalesConfiguration newConfiguration);

        bool DeleteSalesConfiguration(string[] isbns);
        List<SalesConfigurationHistory> GetConfigurationVersionsHistory(string isbn);
        Task<List<ProductConfigWithAccessProvider>> GetAllSalesConfigurations();

        /// <summary>
        /// Returns the desired length list of ISBNs, of the products for which the Sales Configuration has been changed, after the DateTime value passed in.
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <param name="webShop"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<UpdatedSalesConfigInfo> GetUpdatedSaleConfigsInfo(DateTime updatedAfter,
            WebShop webShop, int take);

        Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.SalesConfiguration GetSalesConfigurationForPriceUpates(PriceModelRequest priceModelRequest);

        List<SalesConfigurationRevisionHistoryDto> GetConfigurationVersionsRevisionHistory(string isbn);
        List<SalesConfigurationRevisionHistoryDto> GetGUAConfigurationVersionsRevisionHistory(string isbn);
    }
}