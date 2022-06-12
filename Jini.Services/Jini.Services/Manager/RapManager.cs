using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.Services;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Models;
using Gyldendal.Jini.Services.Utils;
using LoggingManager;

namespace Gyldendal.Jini.Services.Manager
{
    public class RapManager : IRapManager
    {
        #region Private members
        /// <summary>
        ///     Logger Instance
        /// </summary>
        private Logger Logger => Logger.Instance;
        private readonly IRapService _rapService;
        #endregion

        public RapManager(IRapService rapService)
        {
            _rapService = rapService;
        }

        public async Task<List<DigitalProduct>> GetAllDigitalProducts()
        {
            var cachedProducts = (List<DigitalProduct>)MemoryCacher.GetValue(Common.Utils.Utils.RapProductsCacheToken);
            var cachedProductsCount = cachedProducts?.Count ?? 0;
            Logger.LogDebug("Products Cached: " + Util.GetPropertyNameAndValue(() => cachedProductsCount));

            var lastSyncTime = MemoryCacher.GetValue(Common.Utils.Utils.RapLastSyncCacheToken);
            // ReSharper disable once AccessToModifiedClosure
            Logger.LogDebug("Products Cache LastSyncTime: " + Util.GetPropertyNameAndValue(() => lastSyncTime));

            if (cachedProducts != null)
            {
                var newDigitalTitles = await _rapService.GetNewDigitalProducts(lastSyncTime);
                Logger.LogDebug("No of New Titles from RAP: " + Util.GetPropertyNameAndValue(() => newDigitalTitles.Count));
                if (newDigitalTitles.Count > 0)
                {
                    cachedProducts.RemoveAll(t => newDigitalTitles.Any(nt => nt.Isbn == t.Isbn));
                    cachedProducts.AddRange(newDigitalTitles.Where(nt => nt.DeletedDate == null));
                }
                Logger.LogDebug("No of Titles: " + Util.GetPropertyNameAndValue(() => cachedProducts.Count));

            }
            else
            {
                Logger.LogDebug("No Cache");
                cachedProducts = await _rapService.GetDigitalProducts();
                Logger.LogDebug("No of New Titles from RAP: " + Util.GetPropertyNameAndValue(() => cachedProducts.Count));
            }
            lastSyncTime = DateTime.Now.ToString("g");
            MemoryCacher.Add(Common.Utils.Utils.RapLastSyncCacheToken, lastSyncTime);
            MemoryCacher.Add(Common.Utils.Utils.RapProductsCacheToken, cachedProducts);
            return cachedProducts;
        }
        public async Task<List<GradeLevel>> GetGradeLevels(string isbn)
        {
            var o = await _rapService.GetGradeLevels(isbn);
            return o;
        }

        public async Task<List<MediaType>> GetMediaAndMaterialeTypes()
        {
            var mediaAndMaterialeTypes = (List<MediaType>)MemoryCacher.GetValue(Common.Utils.Utils.RapMaterialCacheToken);
            if (mediaAndMaterialeTypes != null) return mediaAndMaterialeTypes;
            mediaAndMaterialeTypes = await _rapService.GetMediaAndMaterialeTypes();
            MemoryCacher.Add(Common.Utils.Utils.RapMaterialCacheToken, mediaAndMaterialeTypes);
            return mediaAndMaterialeTypes;
        }

        public async Task<List<Department>> GetDepartmentsAndEditorials()
        {
            var deparments = (List<Department>) MemoryCacher.GetValue(Common.Utils.Utils.RapAfdelingCacheToken);
            if (deparments != null) return deparments;
            deparments = await _rapService.GetDepartmentsAndEditorials();
            MemoryCacher.Add(Common.Utils.Utils.RapAfdelingCacheToken, deparments);
            return deparments;
        }
    }
}