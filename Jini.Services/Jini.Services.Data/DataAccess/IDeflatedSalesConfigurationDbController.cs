using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.RequestResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Data.DataAccess
{
    public interface IDeflatedSalesConfigurationDbController
    {
        Task<DeflatedSalesConfigurationResponse> GetDeflatedSalesConfiguration(DeflatedSalesConfigurationRequest request, List<string> digitalIsbn);
        List<PeriodPrice> GetPeriodPriceByIsbn(string Isbn);
        bool UpdatePeriodPrice(PeriodPrice model);
    }
}