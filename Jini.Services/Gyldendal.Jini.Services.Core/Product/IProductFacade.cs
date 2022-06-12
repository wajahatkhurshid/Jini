using Gyldendal.Jini.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Core.Product
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