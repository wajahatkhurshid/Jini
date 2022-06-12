using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Contracts;
using Kendo.Mvc;

namespace Gyldendal.Jini.Services.Core.Product.Services
{
    public interface IProductService
    {
        Task AddContainer(string containerId, string payload);
        Task<DigitalProductsResponse> GetProductsAsync(List<FilterDescriptor> request,int page,int pageSize,string sortOrder,string sortField);
        Task<DigitalProduct> GetDigitalProductByIsbnAsync(string isbn);
        Task<DeflatedSalesConfigurationV2Response> GetSalesConfigurations(DeflatedSalesConfigurationRequest request);
        Task<string> GetGradeLevelsByIsbnAsync(string isbn);
    }
}
