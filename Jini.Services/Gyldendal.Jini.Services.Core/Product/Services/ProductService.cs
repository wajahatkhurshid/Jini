using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Jini.Repository.Contracts;
using Gyldendal.Jini.Services.Common;

using Gyldendal.Jini.Services.Common.ErrorHandling;
using Gyldendal.Jini.Services.Common.RequestResponse;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Utils;
using Kendo.Mvc;
using Newtonsoft.Json;

namespace Gyldendal.Jini.Services.Core.Product.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMediaMaterialTypeRepository _mediaMaterialTypeRepository;
        private readonly ILogger _logger;
        public ProductService(IProductRepository productRepository, IMediaMaterialTypeRepository mediaMaterialTypeRepository, ILogger logger)
        {
            _productRepository = productRepository;
            _mediaMaterialTypeRepository = mediaMaterialTypeRepository;
            _logger = logger;
        }
        public async Task AddContainer(string containerId, string payload)
        {
            var container = JsonConvert.DeserializeObject<ProductContainer>(payload);
            var mediaMaterialType = new Contracts.MediaMaterialType();
            if (container == null) throw DeserializationFailure(payload);
            var commaSeparatedGrades = ExtractCommaSeparatedGradesFromEducationSubjectLevels(container.ProductEducationSubjectLevel);
            var mediaMaterialTypeIds = container.ProductMediaMaterialeType.SelectMany(x => x).Select(y => y.NodeId).ToList();
            var department = container.ProductEditorialDivision.SelectMany(d => d).FirstOrDefault();
            if (mediaMaterialTypeIds.Any())
            {
                 mediaMaterialType = await _mediaMaterialTypeRepository.GetMediaMaterialTypeAsync(mediaMaterialTypeIds);
            }
            await _productRepository.UpsertProductAsync(new Data.Product
                {
                    Isbn = container.ProductISBN13,
                    Title = container.ProductTitle,
                    UnderTitle = container.ProductSubtitle,
                    ReleaseDate = container.ProductPublishedDate,
                    GradeLevels = commaSeparatedGrades,
                    GpmMaterialTypeId = mediaMaterialType.MaterialTypeId,
                    GpmMediaTypeId = mediaMaterialType.MediaTypeId,
                    DepartmentCode = department?.NodeId.ToString(),
                    DepartmentName = department?.Name,
                    LastUpdatedDate = DateTime.Now,
                    ContainerInstanceId = container.ContainerInstanceId
                });

        }

        private static string ExtractCommaSeparatedGradesFromEducationSubjectLevels(List<List<GpmNode>> productEducationSubjectLevel)
        {
            var allEducationSubjectLevels = productEducationSubjectLevel.SelectMany(x => x);
            var subjectLevels = allEducationSubjectLevels.Select(y => y.Name);
            subjectLevels = subjectLevels.Select(x => Regex.Match(x, @"(?i)^[\d]{1,2}(?=[\s\S]*klasse$)").Value).Distinct();
            return  string.Join(",", subjectLevels.Where(s => !string.IsNullOrEmpty(s)));
        }

        public async Task<DigitalProductsResponse> GetProductsAsync(List<FilterDescriptor> request,int page, int pageSize, string sortOrder, string sortField)
        {
            return await _productRepository.GetProductsAsync(request, page, pageSize,sortOrder, sortField);
            
        }

        public async Task<DigitalProduct> GetDigitalProductByIsbnAsync(string isbn)
        {
          return  await _productRepository.GetDigitalProductByIsbnAsync(isbn);
        }
        public async Task<DeflatedSalesConfigurationV2Response> GetSalesConfigurations(DeflatedSalesConfigurationRequest request)
        {
            var digitalProducts =await _productRepository.GetSalesConfigurations(request);
            return digitalProducts;

        }

        public async  Task<string> GetGradeLevelsByIsbnAsync(string isbn)
        {
            return await _productRepository.GetGradeLevelsByIsbnAsync(isbn);
        }
        private ApiException DeserializationFailure(string payload)
        {
            var message = $"Failed to deserialize the container with payload: {payload}";
            _logger.Warning(message);
            return new ApiException((ulong)ErrorCodes.UnsupportedContainerType, message, Constants.OrigionatingSystemName);
        }

    }
}
