using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.Dtos;

namespace Gyldendal.Jini.Services.Core.Manager
{
    public interface IJiniManager
    {
        Task<List<ProductConfigWithAccessProvider>> GetAllProductConfigWithAccessProvider();

        void UpdateConfigStatusOfCachedTitle(string isbn);

        List<Data.SalesConfiguration> PopulateSalesConfiguration(SalesConfiguration configuration, int? refSalesConfigTypeCode = null);
    }
}