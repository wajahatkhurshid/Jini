using Gyldendal.Jini.Services.Common.ConfigurationManager;
using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Core.Manager;
using Gyldendal.Jini.Services.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enums = Gyldendal.Jini.Services.Common.Enums;

namespace Gyldendal.Jini.Services.Core.Product
{
    /// <summary>
    ///
    /// </summary>
    public class ProductFacade : IProductFacade
    {
        private readonly IRapManager _rapManager;
        private readonly IJiniManager _jiniManager;
        private readonly ILogger _logger;
        private readonly IJiniConfigurationManager _jiniConfigurationManager;

        public ProductFacade(IRapManager rapManager, IJiniManager jiniManager, ILogger logger, IJiniConfigurationManager jiniConfigurationManager)
        {
            _rapManager = rapManager;
            _jiniManager = jiniManager;
            _logger = logger;
            _jiniConfigurationManager = jiniConfigurationManager;
        }

        public async Task<List<DigitalProduct>> GetProducts()
        {
            //Get All Digital Products from RAP
            var digitalProductsTask = _rapManager.GetAllDigitalProducts();

            //Get Configurations from Jini DB
            var configurationsTask = _jiniManager.GetAllProductConfigWithAccessProvider();
            await Task.WhenAll(digitalProductsTask, configurationsTask);
            var digitalProducts = digitalProductsTask.Result;
            var configurations = configurationsTask.Result;
            digitalProducts = digitalProducts.GroupJoin(configurations,
                rapProducts => rapProducts.Isbn,
                salesConfigs => salesConfigs.Isbn,
                (rapProducts, salesConfigs) => new { DigitalProduct = rapProducts, SalesConfiguration = salesConfigs }
                ).SelectMany(x => x.SalesConfiguration.DefaultIfEmpty()
                    , (x, y) => x).Select(row => new DigitalProduct
                    {
                        Isbn = row.DigitalProduct.Isbn,
                        Authors = row.DigitalProduct.Authors,
                        HasDraft = row.SalesConfiguration.FirstOrDefault(draft => draft.State == (int)Enums.EnumState.Draft) != null,
                        HasTrailAccess = row.SalesConfiguration.FirstOrDefault(trial => trial.TrialLicenseId != null && trial.State == (int)Enums.EnumState.Approved) != null,
                        PublishDate = row.DigitalProduct.PublishDate,
                        DepartmentName = row.DigitalProduct.DepartmentName,
                        SubTitle = row.DigitalProduct.SubTitle,
                        DeletedDate = row.DigitalProduct.DeletedDate,
                        DepartmentCode = row.DigitalProduct.DepartmentCode,
                        HasConfiguration = row.SalesConfiguration.FirstOrDefault(draft => draft.State == (int)Enums.EnumState.Approved) != null,
                        LatestChangeDateInJini = row.SalesConfiguration.OrderBy(x => x.CreatedDate).FirstOrDefault()?.CreatedDate,
                        MaterialTypeCode = row.DigitalProduct.MaterialTypeCode,
                        MaterialTypeName = row.DigitalProduct.MaterialTypeName,
                        MediaTypeCode = row.DigitalProduct.MediaTypeCode,
                        MediaTypeName = row.DigitalProduct.MediaTypeName,
                        SectionCode = row.DigitalProduct.SectionCode,
                        SectionName = row.DigitalProduct.SectionName,
                        Title = row.DigitalProduct.Title,
                        HasProductAccess = row.SalesConfiguration.FirstOrDefault()?.HasProductAccess ?? false,
                        ConfigurationText = GetConfigurationText(row.SalesConfiguration.FirstOrDefault())
                    }).GroupBy(x => x.Isbn).Select(g => g.First()).ToList();
            return digitalProducts;
        }

        private string GetConfigurationText(ProductConfigWithAccessProvider configuration)
        {
            var result = ProductStatus.AwaitingSetup.GetDescription();
            if (configuration == null)
            {
                result = ProductStatus.AwaitingSetup.GetDescription();
            }
            else if (configuration.State == (int)Enums.EnumState.Approved && configuration.HasProductAccess)
            {
                result = ProductStatus.Configured.GetDescription();
            }
            else if (configuration.State == (int)Enums.EnumState.Approved && !configuration.HasProductAccess)
            {
                result = ProductStatus.PendingLoginSetup.GetDescription();
            }
            else if ((configuration.State == null || configuration.State == (int)Enums.EnumState.Draft) && configuration.HasProductAccess)
            {
                result = ProductStatus.AwaitingSalesSetup.GetDescription();
            }

            return result;
        }

        public async Task<List<MediaType>> GetMediaAndMaterialeTypes()
        {
            //Get MediaAndMaterialeTypes from RAP
            return await _rapManager.GetMediaAndMaterialeTypes();
        }

        public async Task<List<Department>> GetDepartmentsAndEditorials()
        {
            //Get GetDepartmentsAndEditorials from RAP
            return await _rapManager.GetDepartmentsAndEditorials();
        }

        public async Task<List<GradeLevel>> GetGradeLevels(string isbn)
        {
            //Get GetDepartmentsAndEditorials from RAP
            return await _rapManager.GetGradeLevels(isbn);
        }

        public async Task<DigitalProduct> GetProductDetails(string isbn)
        {
            //Get All Digital Products from RAP
            var products = await _rapManager.GetAllDigitalProducts();
            return products.FirstOrDefault(p => p.Isbn.Equals(isbn));
        }
    }
}