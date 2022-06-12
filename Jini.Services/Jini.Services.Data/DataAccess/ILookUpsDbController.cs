using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public interface ILookUpsDbController
    {
        List<RefAccessProvider> GetAccessProviders();

        RefAccessProvider GetAccessProvider(int accessProviderCode);

        List<RefSalesForm> GetRefSalesForms();

        List<RefAccessForm> GetRefAccessForms();

        List<RefPeriod> GetRefPeriods(int refSalesFormCode);

        List<RefPeriodUnitType> GetPeriodUnitTypes();

        List<RefPriceModel> GetRefPriceModels(int refAccessFormCode);

        List<RefPeriodUnitType> GetRefPeriodUnitTypes();

        int? GetSellerWebShopId(string sellerName);

        int? GetSellerId(string sellerName);

        Seller GetSellerByName(Api.ShopServices.Contracts.Enumerations.Seller seller);
    }
}