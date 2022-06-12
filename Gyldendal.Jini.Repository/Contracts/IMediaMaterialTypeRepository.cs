using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Contracts;
using MediaType = Gyldendal.Jini.Services.Data.MediaType;
using MaterialType = Gyldendal.Jini.Services.Data.MaterialType;
namespace Gyldendal.Jini.Repository.Contracts
{
   public interface IMediaMaterialTypeRepository
    {
        Task<List<MediaType>> GetMediaAndMaterialTypesAsync();
        Task UpsertMediaTypesAsync(List<MediaType> mediaTypes);
        Task UpsertMaterialTypesAsync(List<MaterialType> materialTypes);
        Task<MediaMaterialType> GetMediaMaterialTypeAsync(List<int> mediaMaterialTypeIds);
    }
}
