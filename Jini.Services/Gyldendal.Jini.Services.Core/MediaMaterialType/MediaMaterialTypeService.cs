using System.Linq;
using Gyldendal.Jini.ExternalClients.Interfaces;
using Gyldendal.Jini.Repository.Contracts;
using System.Threading;
using System.Threading.Tasks;
using MaterialType = Gyldendal.Jini.Services.Data.MaterialType;
using MediaType = Gyldendal.Jini.Services.Data.MediaType;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Core.MediaMaterialType
{
   public  class MediaMaterialTypeService : IMediaMaterialTypeService
    {
        private readonly IMediaMaterialTypeRepository _mediaMaterialTypeRepository;
        private readonly IGpmApiClient _gpmApiClient;
        

        public MediaMaterialTypeService(IMediaMaterialTypeRepository mediaMaterialTypeRepository, IGpmApiClient gpmApiClient)
        {
            _mediaMaterialTypeRepository = mediaMaterialTypeRepository;
            _gpmApiClient = gpmApiClient;
        }

        public async Task<List<Contracts.MediaType>> GetMediaAndMaterialTypesAsync()
        {
            var mediaMaterialTypes =await _mediaMaterialTypeRepository.GetMediaAndMaterialTypesAsync();
            return mediaMaterialTypes.Select(x => new Services.Contracts.MediaType
            {
                MediaTypeCode = x.GpmId.ToString(),
                MediaTypeName = x.Name,
                ListOfMaterialTypes = x.MaterialTypes.Select(y => new Services.Contracts.MaterialType
                {
                    MaterialTypeCode = y.GpmId.ToString(),
                    MaterialTypeName = y.Name
                }).ToArray()
            }).ToList();
        }

        public async Task UpsertMediaMaterialTypesAsync() 
        {
           var taxonomy= await _gpmApiClient.GetTaxonomyAsync(new CancellationToken(false));
           var mediaTypes = taxonomy.TaxonomyNodes.Where(x => x.Level == 0).Select(y => new MediaType
           {
               GpmId = y.NodeId,
               Name = y.Name

           }).ToList();
           await _mediaMaterialTypeRepository.UpsertMediaTypesAsync(mediaTypes);
           var materialTypes = taxonomy.TaxonomyNodes.Where(x => x.Level == 1).Select(y => new MaterialType
           {
               GpmId = y.NodeId,
               Name = y.Name,
               GpmMediaTypeId = y.ParentNodeId.Value
           }).ToList();
           await _mediaMaterialTypeRepository.UpsertMaterialTypesAsync(materialTypes);
        }
    }
}
