using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Data;
using Kendo.Mvc;

namespace Gyldendal.Jini.Repository.Contracts
{
    public interface IProductRepository
    {
        Task<DigitalProductsResponse> GetProductsAsync(List<FilterDescriptor> request, int page, int pageSize, string sortOrder, string sortField);
        Task<DigitalProduct> GetDigitalProductByIsbnAsync(string isbn);
        Task<string> GetGradeLevelsByIsbnAsync(string isbn);
        Task<bool> UpsertProductAsync(Product product);
        Task<DeflatedSalesConfigurationV2Response> GetSalesConfigurations(DeflatedSalesConfigurationRequest request);
    }
}
