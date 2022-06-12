using Gyldendal.Jini.Services.Common.ConfigurationManager;
using Gyldendal.Jini.Services.Common.Services;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Utils;
using Gyldendal.Jini.Utilities.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Core.Manager
{
    public class RapManager : IRapManager
    {
        #region Private members

        /// <summary>
        ///     Logger Instance
        /// </summary>
        private readonly ILogger _logger;

        private readonly IRapService _rapService;
        private readonly ICacher _cacher;
        private readonly IJiniConfigurationManager _jiniConfigurationManager;

        #endregion Private members

        public RapManager(IJiniConfigurationManager jiniConfigurationManager, IRapService rapService, ICacher cacher, ILogger logger)
        {
            _jiniConfigurationManager = jiniConfigurationManager;
            _rapService = rapService;
            _cacher = cacher;
            _logger = logger;
        }

        public async Task<List<DigitalProduct>> GetAllDigitalProducts()
        {
            var cachedProducts = (List<DigitalProduct>)MemoryCacher.GetValue(Common.Utils.Utils.RapProductsCacheToken);
            var cachedProductsCount = cachedProducts?.Count ?? 0;
            _logger.Debug("Products Cached: " + _logger.GetPropertyNameAndValue(() => cachedProductsCount), isGdprSafe: true);

            if (cachedProductsCount <= 0)
            {
                var departmentsToExclude = _jiniConfigurationManager.DepartmentsToExclude.Split(',');
                var sectionsToExclude = _jiniConfigurationManager.SectionsToExclude.Split(',');

                _logger.Debug("No Cache");
                cachedProducts = await _rapService.GetDigitalProducts();
                cachedProducts = cachedProducts.Where(x => !(sectionsToExclude.Contains(x.SectionCode) ||
                                                             departmentsToExclude.Contains(x.DepartmentCode))).ToList();
                _logger.Debug("No of New Titles from RAP: " + _logger.GetPropertyNameAndValue(() => cachedProducts.Count), isGdprSafe: true);
                SaveProductsInCache(cachedProducts);
            }
            else
            {
                var lastSyncTime = MemoryCacher.GetValue(Common.Utils.Utils.RapLastSyncCacheToken);
                // ReSharper disable once AccessToModifiedClosure
                _logger.Debug("Products Cache LastSyncTime: " + _logger.GetPropertyNameAndValue(() => lastSyncTime), isGdprSafe: true);

                int productCacheSyncTime;
                if (!int.TryParse(_jiniConfigurationManager.CacheDuration, out productCacheSyncTime))
                    productCacheSyncTime = 5;

                DateTime lastSyncTimeValue;
                if (!DateTime.TryParse((string)lastSyncTime, out lastSyncTimeValue))
                    lastSyncTimeValue = DateTime.MinValue;

                var itsTimeToSync = lastSyncTimeValue.AddMinutes(productCacheSyncTime) < DateTime.UtcNow;

                if (itsTimeToSync)
                {
                    var newDigitalTitles = await _rapService.GetNewDigitalProducts(lastSyncTime);
                    _logger.Debug("No of New Titles from RAP: " + _logger.GetPropertyNameAndValue(() => newDigitalTitles.Count), isGdprSafe: true);
                    if (newDigitalTitles.Count > 0)
                    {
                        cachedProducts.RemoveAll(t => newDigitalTitles.Any(nt => nt.Isbn == t.Isbn));
                        cachedProducts.AddRange(newDigitalTitles.Where(nt => nt.DeletedDate == null));
                        SaveProductsInCache(cachedProducts);
                    }
                    _logger.Debug("No of Titles: " + _logger.GetPropertyNameAndValue(() => cachedProducts.Count), isGdprSafe: true);
                }
            }

            return cachedProducts;
        }

        private void SaveProductsInCache(List<DigitalProduct> cachedProducts)
        {
            MemoryCacher.Add(Common.Utils.Utils.RapLastSyncCacheToken, DateTime.UtcNow.ToString("g"));
            MemoryCacher.Add(Common.Utils.Utils.RapProductsCacheToken, cachedProducts);
        }

        /// <summary>
        /// Fetching gradeLevels from RapMeta service and its cache handling.
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public async Task<List<GradeLevel>> GetGradeLevels(string isbn)
        {
            var result = _cacher.GetValue<List<GradeLevel>>(isbn);
            if (result != null)
                return result;

            var gradeLevels = await _rapService.GetGradeLevels(isbn);

            int cacheDurationInMin;
            if (!int.TryParse(_jiniConfigurationManager.CacheDuration, out cacheDurationInMin))
                cacheDurationInMin = 30; // setting 30 as default value

            _cacher.Add(isbn, gradeLevels, new TimeSpan(0, 0, cacheDurationInMin, 0));

            return gradeLevels;
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
            var deparments = (List<Department>)MemoryCacher.GetValue(Common.Utils.Utils.RapAfdelingCacheToken);
            if (deparments != null) return deparments;

            var departmentsToExclude = _jiniConfigurationManager.DepartmentsToExclude.Split(',');
            var sectionsToExclude = _jiniConfigurationManager.SectionsToExclude.Split(',');

            deparments = await _rapService.GetDepartmentsAndEditorials();

            deparments = deparments.Where(x => !departmentsToExclude.Contains(x.Code)).ToList();
            foreach (var deparment in deparments)
            {
                if (deparment.Code.Equals("ORG300000490"))
                {
                    deparment.Sections = deparment.Sections.Where(x => !sectionsToExclude.Contains(x.SectionCode))
                        .ToList();
                }
            }

            MemoryCacher.Add(Common.Utils.Utils.RapAfdelingCacheToken, deparments);
            return deparments;
        }
    }
}