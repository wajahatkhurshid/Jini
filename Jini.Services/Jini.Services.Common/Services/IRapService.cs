using Gyldendal.Jini.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Common.Services
{
    public interface IRapService
    {
        Task<List<GradeLevel>> GetGradeLevels(string isbn);

        Task<List<DigitalProduct>> GetDigitalProducts();

        Task<List<DigitalProduct>> GetNewDigitalProducts(object lastSyncTime);

        Task<List<Department>> GetDepartmentsAndEditorials();

        Task<List<MediaType>> GetMediaAndMaterialeTypes();
    }
}