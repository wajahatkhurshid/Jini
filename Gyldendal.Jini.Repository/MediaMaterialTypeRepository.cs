using Gyldendal.Jini.Repository.Contracts;
using Gyldendal.Jini.Services.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Contracts;
using MaterialType = Gyldendal.Jini.Services.Data.MaterialType;
using MediaType = Gyldendal.Jini.Services.Data.MediaType;
using Gyldendal.Jini.Services.Common.ConfigurationManager;

namespace Gyldendal.Jini.Repository
{
    public class MediaMaterialTypeRepository :  IMediaMaterialTypeRepository
    {
        private readonly IRepository<MediaType> _mediaRepository;
        private readonly IRepository<MaterialType> _materialRepository;

        private readonly string _connectionString;
        public MediaMaterialTypeRepository(Jini_Entities context, IJiniConfigurationManager jiniConfigurationManager)
        {
            this._mediaRepository = new BaseRepository<MediaType>(context);
            this._materialRepository = new BaseRepository<MaterialType>(context);
            _connectionString = jiniConfigurationManager.ConnectionString;
        }

        public async  Task<List<MediaType>> GetMediaAndMaterialTypesAsync()
        {
            return await _mediaRepository.GetAllAsync();
        }
        public async Task UpsertMediaTypesAsync(List<MediaType> mediaTypes)
        {
            foreach (var media in mediaTypes)
            {
               await _mediaRepository.UpsertAsync(media);
            } 
        }
        public async Task UpsertMaterialTypesAsync(List<MaterialType> materialTypes)
        {
            foreach (var material in materialTypes)
            {
                await _materialRepository.UpsertAsync(material);
            }
        }

        public async Task<MediaMaterialType> GetMediaMaterialTypeAsync(List<int> mediaMaterialTypeIds)
        {
            var mediaMaterialType = new MediaMaterialType();
            using (var context = new Jini_Entities(_connectionString))
            {
                var mediaType = await context.MediaTypes.FindAsync(mediaMaterialTypeIds[0]);
                if (!(mediaType is null))
                {
                    mediaMaterialType.MediaTypeId = mediaType.GpmId;
                }
                else
                {
                    var materialType = await context.MaterialTypes.FindAsync(mediaMaterialTypeIds[0]);
                    if (materialType == null)
                        return mediaMaterialType;
                    mediaMaterialType.MediaTypeId = materialType.GpmMediaTypeId;
                    mediaMaterialType.MaterialTypeId = materialType.GpmId;
                    return mediaMaterialType;
                }
                var materialTypeId = await context.MaterialTypes.FindAsync(mediaMaterialTypeIds[1]);
                if (!(materialTypeId is null))
                {
                    mediaMaterialType.MaterialTypeId = materialTypeId.GpmId;
                }
            }
            return mediaMaterialType;
        }
    }
}
