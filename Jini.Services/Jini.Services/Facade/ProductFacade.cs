using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Core.Manager;
using Gyldendal.Jini.Services.Utils;
using Enums = Gyldendal.Jini.Services.Common.Enums;

namespace Gyldendal.Jini.Services.Facade
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductFacade : IProductFacade
    {
        private readonly IRapManager _rapManager;
        private readonly IJiniManager _jiniManager;
        private readonly ILogger _logger;
        private readonly ISettingsWrapper _settings;

        public ProductFacade(IRapManager rapManager, IJiniManager jiniManager, ILogger logger, ISettingsWrapper settings)
        {
            _rapManager = rapManager;
            _jiniManager = jiniManager;
            _logger = logger;
            _settings = settings;
        }

        public async Task<List<DigitalProduct>> GetProducts()
        {
            //Get All Digital Products from RAP
            var digitalProducts = await _rapManager.GetAllDigitalProducts();

            //Get Configurations from Jini DB
            var configurations = _jiniManager.GetAllConfigurations();

            foreach (var c in configurations)
            {
                var digitalTitle = digitalProducts.FirstOrDefault(dt => dt.Isbn.Equals(c.Isbn));
                if (digitalTitle != null)
                {
                    digitalTitle.HasConfiguration =
                        configurations.Any(
                            config =>
                                config.Isbn == digitalTitle.Isbn && config.State == Enums.ToInt(Enums.EnumState.Approved));
                    digitalTitle.HasDraft =
                        configurations.Any(
                            config => config.Isbn == digitalTitle.Isbn && config.State == Enums.ToInt(Enums.EnumState.Draft));
                    //digitalTitle.ConfigurationText = _settings.InactiveConfiguration; // Redundant! NR

                    if (digitalTitle.HasConfiguration)
                    {
                        digitalTitle.ConfigurationText = _settings.ActiveConfiguration;
                    }
                    else
                    {
                        if (digitalTitle.HasDraft)
                        {
                            digitalTitle.ConfigurationText = "Påbegyndt";
                        }
                        else
                        {
                            digitalTitle.ConfigurationText = _settings.InactiveConfiguration;
                        }
                    }
                }
                else
                {
                    _logger.LogError("Configured Product not found in Rap Product List", new Exception("Error occured in Product Facade in Jini Services"));
                }
            }

            return digitalProducts;
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