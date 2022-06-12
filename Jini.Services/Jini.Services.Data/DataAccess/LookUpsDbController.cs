using Gyldendal.Jini.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public class LookUpsDbController : ILookUpsDbController
    {
        private readonly ILogger _logger;

        private readonly Jini_Entities _dbContext;

        public LookUpsDbController(ILogger logger, Jini_Entities dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public List<RefAccessProvider> GetAccessProviders()
        {
            return _dbContext.RefAccessProviders.ToList();
        }

        public RefAccessProvider GetAccessProvider(int accessProviderCode)
        {
            return _dbContext.RefAccessProviders.FirstOrDefault(x => x.Code == accessProviderCode);
        }

        public List<RefSalesForm> GetRefSalesForms()
        {
            return _dbContext.RefSalesForms.ToList();
        }

        public List<RefAccessForm> GetRefAccessForms()
        {
            return _dbContext.RefAccessForms.ToList();
        }

        public List<RefPeriod> GetRefPeriods(int refSalesFormCode)
        {
            return _dbContext.RefPeriods.Include("RefPeriodUnitType").ToList();
        }

        public List<RefPeriodUnitType> GetPeriodUnitTypes()
        {
            return _dbContext.RefPeriodUnitTypes.ToList();
        }

        public List<RefPriceModel> GetRefPriceModels(int refAccessFormCode)
        {
            return _dbContext.RefPriceModels.Where(rp => rp.RefAccessForm.Code == refAccessFormCode).ToList();
        }

        public List<RefPeriodUnitType> GetRefPeriodUnitTypes()
        {
            return _dbContext.RefPeriodUnitTypes.ToList();
        }

        #region Seller

        public int? GetSellerWebShopId(string sellerName)
        {
            return _dbContext.Sellers.FirstOrDefault(x => x.Name.Trim().Equals(sellerName.Trim(), StringComparison.CurrentCultureIgnoreCase))?.WebShopId;
        }

        public int? GetSellerId(string sellerName)
        {
            return _dbContext.Sellers.FirstOrDefault(x => x.Name.Trim().Replace(" ","").Equals(sellerName.Trim().Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase))?.Id;
        }

        public Seller GetSellerByName(Api.ShopServices.Contracts.Enumerations.Seller seller)
        {
            return _dbContext.Sellers.FirstOrDefault(sc => sc.Id.Equals((int)seller));
        }

        #endregion Seller
    }
}