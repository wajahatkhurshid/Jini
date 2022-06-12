using SaleConfiguration_Operations.DataAccess;
using System.Data.Entity;
using System.Linq;

namespace SaleConfiguration_Operations
{
    public class SaleConfigurationDataAccess
    {
        private SalesConfigurationEntities _dbContext;

        public SalesConfiguration GetSalesConfiguration(int id)
        {
            SalesConfiguration salesConfiguration;
            using (_dbContext = new SalesConfigurationEntities())
            {
                salesConfiguration = _dbContext.SalesConfigurations.Where(x => x.Id == id)
                    .Include("SalesForm")
                    .Include("SalesForm.RefSalesForm")
                    .Include("Seller")
                    .Include("AccessForms")
                    .Include("AccessForms.PriceModels")
                    .Include("AccessForms.PriceModels.RefPriceModel")
                    .Include("AccessForms.RefAccessForm")
                    .Include("AccessForms.PeriodPrices")
                    .Include("AccessForms.PeriodPrices.RefPeriodUnitType")
                    .FirstOrDefault();
            }
            return salesConfiguration;
        }

        public void UpdateSalesConfigurationPrice(PeriodPrice updatedPeriodPrice)
        {
            using (_dbContext = new SalesConfigurationEntities())
            {
                _dbContext.PeriodPrices.Attach(updatedPeriodPrice);
                _dbContext.Entry(updatedPeriodPrice).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
        }
    }
}