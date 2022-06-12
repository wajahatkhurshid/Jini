using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Models;

namespace Gyldendal.Jini.Services.Manager
{
    public interface IRapManager
    {
        Task<List<DigitalProduct>> GetAllDigitalProducts();
        Task<List<GradeLevel>> GetGradeLevels(string isbn);
        Task<List<MediaType>> GetMediaAndMaterialeTypes();
        Task<List<Department>> GetDepartmentsAndEditorials();
    }
}