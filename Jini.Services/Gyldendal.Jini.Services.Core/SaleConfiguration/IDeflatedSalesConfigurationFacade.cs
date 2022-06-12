using Gyldendal.Jini.Services.Common.RequestResponse;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Core.SaleConfiguration
{
    public interface IDeflatedSalesConfigurationFacade
    {
        Task<DeflatedSalesConfigurationResponse> GetDeflatedSalesConfiguration(DeflatedSalesConfigurationRequest request);
        bool SaveDeflatedPrices(PriceModelRequest request);


    }
}
