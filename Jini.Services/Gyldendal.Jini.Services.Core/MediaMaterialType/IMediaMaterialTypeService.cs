using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Core.MediaMaterialType
{
 public   interface IMediaMaterialTypeService
 {
      Task UpsertMediaMaterialTypesAsync();
      Task<List<Contracts.MediaType>> GetMediaAndMaterialTypesAsync();
    }
}
