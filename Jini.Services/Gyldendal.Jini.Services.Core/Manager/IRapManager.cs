using Gyldendal.Jini.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Core.Manager
{
    public interface IRapManager
    {
        Task<List<DigitalProduct>> GetAllDigitalProducts();

        Task<List<GradeLevel>> GetGradeLevels(string isbn);

        Task<List<MediaType>> GetMediaAndMaterialeTypes();

        Task<List<Department>> GetDepartmentsAndEditorials();
    }
}