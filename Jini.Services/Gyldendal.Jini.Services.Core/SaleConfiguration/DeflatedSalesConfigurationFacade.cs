using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Core.Manager;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Core.SaleConfiguration
{
    public class DeflatedSalesConfigurationFacade : IDeflatedSalesConfigurationFacade
    {
        private readonly IDeflatedSalesConfigurationDbController _deflatedSalesConfigurationDbController;
        private IRapManager _rapManager;
        private List<string> _sectionCodes;
        private List<string> _departmentCodes;
        private List<string> _materialTypeCodes;
        private List<string> _mediaTypeCodes;

        private ISaleConfigurationDbController _saleConfigurationDbController;
        private IJiniManager _jiniManager;
        private readonly ILogger _logger;
        public DeflatedSalesConfigurationFacade(IDeflatedSalesConfigurationDbController deflatedSalesConfigurationDbController,
            IRapManager rapManager, ISaleConfigurationDbController configurationDbController, IJiniManager jiniManager, ILogger logger)
        {
            _deflatedSalesConfigurationDbController = deflatedSalesConfigurationDbController;
            _rapManager = rapManager;
            _saleConfigurationDbController = configurationDbController;
            _jiniManager = jiniManager;
            _logger = logger;
        }

        /// <summary>
        /// Get Deflated sales configuration b
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<DeflatedSalesConfigurationResponse> GetDeflatedSalesConfiguration(DeflatedSalesConfigurationRequest request)
        {
            await GetDepratmentCodes();
            await GetMaterialTypeCodes();

            var isMediaTypeFilterApplied = (request.MaterialTypeCodes != null);
            if (isMediaTypeFilterApplied)
            {
                _materialTypeCodes = request.MaterialTypeCodes.ToList();
                _mediaTypeCodes = request.MediaTypeCodes.ToList();
            }

            var isSectionFilterApplied = (request.DepartmentCodes != null);
            if (isSectionFilterApplied)
            {
                _sectionCodes = request.SectionCodes.ToList();
                _departmentCodes = request.DepartmentCodes.ToList();

            }

            TimeZoneInfo tzi = TimeZoneInfo.Local;//FindSystemTimeZoneById("Central European Standard Time");

            DateTime? releaseStartDate = !string.IsNullOrEmpty(request.ReleaseStartDate)
            ? TimeZoneInfo.ConvertTime(Convert.ToDateTime(request.ReleaseStartDate), tzi)
            : (DateTime?)null;

            var releaseEndDate = !string.IsNullOrEmpty(request.ReleaseEndDate)
            ? TimeZoneInfo.ConvertTime(Convert.ToDateTime(request.ReleaseEndDate), tzi)
            : (DateTime?)null;

            var digitalProducts = (await _rapManager.GetAllDigitalProducts())
                .Where(x => (request.Isbn == "" || x.Isbn.Contains(request.Isbn))
                            &&
                            (request.Title == "" || (!string.IsNullOrEmpty(x.Title) &&
                                                     x.Title.Trim().ToLower()
                                                         .Contains(request.Title.Trim().ToLower()))) &&

                            // (!isMediaTypeFilterApplied || (_materialTypeCodes.Contains(x.MaterialTypeCode) && _mediaTypeCodes.Contains(x.MediaTypeCode))) &&
                            // (!isSectionFilterApplied || (_sectionCodes.Contains(x.SectionCode) && _departmentCodes.Contains(x.DepartmentCode))) &&
                            //Relase Date Check start here
                            (!releaseStartDate.HasValue ||
                             (x.PublishDate.HasValue &&
                              x.PublishDate.Value.Date >= releaseStartDate.Value.Date &&
                              x.PublishDate.Value.Date <= releaseEndDate.Value.Date)

                            ) && !x.DeletedDate.HasValue)
                //Rlease Date Check ends here
                //End of Where Clause
                .AsEnumerable();

            List<DigitalProduct> filteredProducts = new List<DigitalProduct>();

            if (!request.IsAllMediaTypesChecked)
            {
                if (request.MediaMaterialTypes != null && request.MediaMaterialTypes.Length > 0)
                {
                    foreach (var mmt in request.MediaMaterialTypes)
                    {
                        var fetchedRecords = digitalProducts.Where(x =>
                            (x.MediaTypeCode == mmt.MediaTypeCode && x.MaterialTypeCode == mmt.MaterialTypeCode)).ToList();
                        filteredProducts.AddRange(fetchedRecords);

                    }

                    digitalProducts = filteredProducts;
                }
                else
                {
                    digitalProducts = new List<DigitalProduct>();
                }
            }


            if (!request.IsAllDepartmentChecked)
            {
                var filteredProductsByDepartment = new List<DigitalProduct>();

                if (request.DepartmentsSections != null && request.DepartmentsSections.Length > 0)
                {
                    foreach (var ds in request.DepartmentsSections)
                    {
                        var fetchedRecords = digitalProducts.Where(x =>
                            (x.DepartmentCode == ds.DepartmentCode && x.SectionCode == ds.SectionCode)).ToList();
                        filteredProductsByDepartment.AddRange(fetchedRecords);

                    }

                    digitalProducts = filteredProductsByDepartment;
                }
                else
                {
                    digitalProducts = new List<DigitalProduct>();
                }
            }

            var digitalIsbn = digitalProducts.OrderBy(x => x.Isbn).Select(x => x.Isbn).ToList();
            var salesConfigObject = await _deflatedSalesConfigurationDbController.GetDeflatedSalesConfiguration(request, digitalIsbn);
            var totalCount = salesConfigObject.Total;
            var deflatedSalesConfigurations = salesConfigObject.Data;
            var totalProducts = salesConfigObject.ProductCount;
            
            foreach (var salesConfiguration in deflatedSalesConfigurations)
            {
                var rapData = digitalProducts.FirstOrDefault(x => x.Isbn == salesConfiguration.Isbn);

                if (rapData is null) continue;

                salesConfiguration.Title = rapData.Title;
                salesConfiguration.ReleaseDate = rapData.PublishDate;
            }


            deflatedSalesConfigurations = SortData(deflatedSalesConfigurations, request.SortField, request.SortOrder);
            return new DeflatedSalesConfigurationResponse()
            {
                Total = totalCount,
                ProductCount = totalProducts,
                Data = deflatedSalesConfigurations
            };
        }

        public bool SaveDeflatedPrices(PriceModelRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Isbn))
            {

                var configuration = GetSalesConfigurationForPriceUpates(request);

                try
                {
                    var salesConfigurations = _saleConfigurationDbController.GetSalesConfigurations(configuration.Isbn, configuration.Seller,request.RefSalesconfigTypeCode);
                    configuration.CreatedBy = request.DeflatedPrice.FirstOrDefault()?.CreatedBy;
                    var newSalesConfigurations = _jiniManager.PopulateSalesConfiguration(configuration,request.RefSalesconfigTypeCode);
                    // No Sales configuration Exists
                    bool result;

                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (salesConfigurations.Count == 0)
                    {
                        result = _saleConfigurationDbController.CreateSalesConfiguration(newSalesConfigurations.FirstOrDefault());
                    }

                    // If sales configuration Exists then Update
                    else
                    {
                        result = _saleConfigurationDbController.UpdateBulkSalesConfiguration(salesConfigurations, newSalesConfigurations.FirstOrDefault());
                    }
                    if (result)
                    {
                        if (configuration.Approved)
                        {
                            _jiniManager.UpdateConfigStatusOfCachedTitle(configuration.Isbn);
                        }

                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex, isGdprSafe: false);

                }

            }

            return true;
        }


        private async Task GetDepratmentCodes()
        {
            var departments = await _rapManager.GetDepartmentsAndEditorials();
            _sectionCodes = new List<string>();
            _departmentCodes = new List<string>();
            foreach (var department in departments)
            {
                foreach (var section in department.Sections)
                {
                    _sectionCodes.Add(section.SectionCode.Trim());
                }
                _departmentCodes.Add(department.Code);

            }
        }

        private async Task GetMaterialTypeCodes()
        {
            _materialTypeCodes = new List<string>();
            _mediaTypeCodes = new List<string>();
            var mediaTypes = await _rapManager.GetMediaAndMaterialeTypes();
            foreach (var mediaType in mediaTypes)
            {
                foreach (var materialType in mediaType.ListOfMaterialTypes)
                {
                    _materialTypeCodes.Add(materialType.MaterialTypeCode);
                }
                _mediaTypeCodes.Add(mediaType.MediaTypeCode);
            }
        }

        private List<DeflatedSalesConfigurationDto> SortData(List<DeflatedSalesConfigurationDto> data, string sortName, string order)
        {
            if (!string.IsNullOrEmpty(sortName) && !string.IsNullOrEmpty(order))
            {
                if (order == "asc")
                {
                    switch (sortName)
                    {
                        case "Isbn":
                            data = data.OrderBy(x => x.Isbn).ToList();
                            break;
                        case "Title":
                            data = data.OrderBy(x => x.Title).ToList();
                            break;
                        //case "Salgsform":
                        //    data = data.OrderBy(x => x.RefSalesDisplayName).ToList();
                        //    break;
                        case "RefSalesDisplayName":
                            data = data.OrderBy(x => x.RefSalesDisplayName).ToList();
                            break;
                        case "RefPeriodTypeDisplayName":
                            data = data.OrderBy(x => x.RefPeriodTypeDisplayName).ToList();
                            break;
                        case "ReleaseDate":
                            data = data.OrderBy(x => x.ReleaseDate).ToList();
                            break;
                        case "LastModified":
                            data = data.OrderBy(x => x.LastModified).ToList();
                            break;

                        case "UnitPriceVat":
                            data = data.OrderBy(x => x.UnitPriceVat).ToList();
                            break;
                        case "UnitPrice":
                            data = data.OrderBy(x => x.UnitPrice).ToList();
                            break;

                    }
                }

                if (order == "desc")
                {
                    switch (sortName)
                    {
                        case "Isbn":
                            data = data.OrderByDescending(x => x.Isbn).ToList();
                            break;
                        case "Title":
                            data = data.OrderByDescending(x => x.Title).ToList();
                            break;
                        //case "Salgsform":
                        //    data = data.OrderByDescending(x => x.Isbn).ToList();
                        //    break;
                        case "RefSalesDisplayName":
                            data = data.OrderByDescending(x => x.RefSalesDisplayName).ToList();
                            break;
                        case "RefPeriodTypeDisplayName":
                            data = data.OrderByDescending(x => x.RefPeriodTypeDisplayName).ToList();
                            break;
                        case "ReleaseDate":
                            data = data.OrderByDescending(x => x.ReleaseDate).ToList();
                            break;
                        case "LastModified":
                            data = data.OrderByDescending(x => x.LastModified).ToList();
                            break;
                        case "UnitPrice":
                            data = data.OrderByDescending(x => x.UnitPrice).ToList();
                            break;
                        case "UnitPriceVat":
                            data = data.OrderByDescending(x => x.UnitPriceVat).ToList();
                            break;
                    }
                }
            }
            else
            {
                data = data.OrderByDescending(x => x.LastModified).ToList();
            }
           
            return data.OrderBy(x=>x.Isbn).ToList();
        }

        private SalesConfiguration GetSalesConfigurationForPriceUpates(PriceModelRequest request)
        {
            return _saleConfigurationDbController.GetSalesConfigurationForPriceUpates(request);
        }
    }

}