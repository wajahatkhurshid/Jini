using Gyldendal.Jini.Services.Common.Services.RapServiceContract;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Common.Services
{
    public class RapService : IRapService
    {
        private readonly IServiceHelper _serviceHelper;

        public RapService(IServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        public async Task<List<GradeLevel>> GetGradeLevels(string isbn)
        {
            var gradeLevels = await _serviceHelper.GetAsync<List<GradeLevel>>(ServiceHelper.RapMetaServiceUrl, "ProductGradeLevel", "GetGradeLevels", $"?isbn={isbn}");
            return gradeLevels;
        }

        public async Task<List<DigitalProduct>> GetDigitalProducts()
        {
            var digitalProducts = await _serviceHelper.GetAsync<List<DigitalProductResponse>>(ServiceHelper.RapMetaServiceUrl, "Product", "GetDigitalTitles/null");
            return GetDigitalProducts(digitalProducts);
        }

        public async Task<List<DigitalProduct>> GetNewDigitalProducts(object lastSyncTime)
        {
            var newDigitalTitles = await _serviceHelper.GetAsync<List<DigitalProductResponse>>(ServiceHelper.RapMetaServiceUrl, "Product",
                        $"GetDigitalTitles/lastSyncTime");

            return GetDigitalProducts(newDigitalTitles);
        }

        public async Task<List<Department>> GetDepartmentsAndEditorials()
        {
            var departments = await _serviceHelper.GetAsync<List<Department>>(ServiceHelper.RapMetaServiceUrl, "Organization",
                        "GetDigitalTitleDepartments");
            return departments;
        }

        public async Task<List<MediaType>> GetMediaAndMaterialeTypes()
        {
            var mediaTypes = await _serviceHelper.GetAsync<List<MediaType>>(ServiceHelper.RapMetaServiceUrl, "MediaType", "GetDigitalTitleMediaType");
            return mediaTypes;
        }

        #region PrivateMethods

        private List<DigitalProduct> GetDigitalProducts(List<DigitalProductResponse> responses)
        {
            List<DigitalProduct> digitalProducts = new List<DigitalProduct>();
            foreach (var product in responses)
            {
                digitalProducts.Add(new DigitalProduct()
                {
                    Title = product.Title,
                    SubTitle = product.UnderTitle,
                    Isbn = product.ISBN13,
                    DepartmentCode = product.Department.Code,
                    DepartmentName = product.Department.Name,
                    SectionCode = product.Section.Code,
                    SectionName = product.Section.Name,
                    MaterialTypeCode = product.MaterialType.MaterialTypeCode,
                    MaterialTypeName = product.MaterialType.MaterialTypeName,
                    MediaTypeCode = product.MediaType.MediaTypeCode,
                    MediaTypeName = product.MediaType.MediaTypeName,
                    Authors = product.Authors,
                    PublishDate = product.ReleaseDate
                });
            }
            return digitalProducts;
        }

        #endregion PrivateMethods
    }
}