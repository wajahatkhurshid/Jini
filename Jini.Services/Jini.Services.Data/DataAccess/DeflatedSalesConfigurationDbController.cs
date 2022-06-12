using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Data.MappingHelper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public class DeflatedSalesConfigurationDbController : IDeflatedSalesConfigurationDbController
    {
        private readonly Jini_Entities _dbContext;

        public DeflatedSalesConfigurationDbController(Jini_Entities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeflatedSalesConfigurationResponse> GetDeflatedSalesConfiguration(DeflatedSalesConfigurationRequest request, List<string> digitalIsbn)
        {
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            DateTime? modifiedStartDate = !string.IsNullOrEmpty(request.ModifiedStartDate)
                ? TimeZoneInfo.ConvertTime(Convert.ToDateTime(request.ModifiedStartDate),tzi)
                : (DateTime?)null;
            var modifiedEndDate = !string.IsNullOrEmpty(request.ModifiedEndDate)
                ? TimeZoneInfo.ConvertTime(Convert.ToDateTime(request.ModifiedEndDate),tzi)
                : (DateTime?)null;


            var query = modifiedStartDate.HasValue && modifiedEndDate.HasValue ?
                _dbContext.DeflatedSalesConfigurationViews.AsNoTracking()
                .Where(x => (
                     //Modified Date Check start here

                     (EntityFunctions.TruncateTime(x.CreatedDate) >= EntityFunctions.TruncateTime(modifiedStartDate) &&
                      EntityFunctions.TruncateTime(x.CreatedDate) <= EntityFunctions.TruncateTime(modifiedEndDate))
                     //Modified Date check ends here 
                     &&
                      (digitalIsbn.Contains(x.Isbn))
                    ))
                :
            _dbContext.DeflatedSalesConfigurationViews.AsNoTracking()
                .Where(x => (
                     (digitalIsbn.Contains(x.Isbn))
                    ));

            // var totalCount = await query.CountAsync();

            //request.Page = totalCount < request.PageSize ? 0 : request.Page;

            try
            {
                return await GetsalesConfigurations(query, request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<DeflatedSalesConfigurationResponse> GetsalesConfigurations(IQueryable<DeflatedSalesConfigurationView> source,
            DeflatedSalesConfigurationRequest request)
        {
            var salesCongfigurations = source.ToList();
            if (request.SalesConfigurationStates != null && request.SalesConfigurationStates.Any())
            {
                if (request.SalesConfigurationStates.Contains("1") && !request.SalesConfigurationStates.Contains("2"))
                {
                    salesCongfigurations = salesCongfigurations
                     .Where(x => x.IsExternalLogin == 1 || x.ProductAccessProvider == 1).ToList();
                }
                else if (request.SalesConfigurationStates.Contains("2") && !request.SalesConfigurationStates.Contains("1"))
                {
                    salesCongfigurations = salesCongfigurations
                     .Where(x => x.IsExternalLogin == 0 && x.ProductAccessProvider == 0)
                     .ToList();
                }
            }
            var totalCount = salesCongfigurations.Count();
            var totalProducts = salesCongfigurations.GroupBy(x => x.Isbn).Count();
            
            var salesConfigurations = salesCongfigurations
                .OrderByDescending(x => x.Isbn)
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

            var objectToRetrun = new DeflatedSalesConfigurationResponse
            {
                Data = salesConfigurations.Select(x => x.MapDeflatedSalesConfigurationToDto()).ToList(),
                Total = totalCount,
                ProductCount = totalProducts
            };

            return objectToRetrun;
        }



        public List<PeriodPrice> GetPeriodPriceByIsbn(string Isbn)
        {
            //PeriodPrices =>pp
            var pp = (from sc in _dbContext.SalesConfigurations
                      join af in _dbContext.AccessForms on sc.Id equals af.SalesConfigurationId
                      join p in _dbContext.PeriodPrices on af.Id equals p.AccessFormId
                      select new PeriodPrice()
                      {
                          Id = p.Id,
                          UnitValue = p.UnitValue,
                          RefPeriodTypeCode = p.RefPeriodTypeCode,
                          AccessFormId = p.AccessFormId,
                          Currency = p.Currency,
                          UnitPrice = p.UnitPrice,
                          UnitPriceVat = p.UnitPriceVat,
                          VatValue = p.VatValue,
                          IsCustomPeriod = p.IsCustomPeriod,

                      }

                ).ToList();

            return pp;
        }

        public bool UpdatePeriodPrice(PeriodPrice model)
        {
            try
            {
                _dbContext.Entry(model).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
