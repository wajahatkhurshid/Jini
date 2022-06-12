using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;

namespace Gyldendal.Jini.Services.Facade
{
    public interface IProductFacade
    {
        Task<List<DigitalProduct>> GetProducts();
        Task<List<MediaType>> GetMediaAndMaterialeTypes();
        Task<List<Department>> GetDepartmentsAndEditorials();
        Task<List<GradeLevel>> GetGradeLevels(string isbn);
        Task<DigitalProduct> GetProductDetails(string isbn);
    }
}