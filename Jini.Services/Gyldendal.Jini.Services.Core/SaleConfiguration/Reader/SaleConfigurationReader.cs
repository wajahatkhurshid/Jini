using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Jini.Services.Common;
using Gyldendal.Jini.Services.Common.ErrorHandling;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Core.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Core.Product.Services;
using Gyldendal.Jini.Services.Core.Seller;
using Gyldendal.Jini.Services.Data.DataAccess;
using DBSaleConfiguration = Gyldendal.Jini.Services.Data.SalesConfiguration;
using Enums = Gyldendal.Jini.Services.Common.Enums;
using SalesConfiguration = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.SalesConfiguration;
using Gyldendal.Jini.Services.Utils;

namespace Gyldendal.Jini.Services.Core.SaleConfiguration.Reader
{
    public class SaleConfigurationReader : ISaleConfigurationReader
    {
        private readonly ILogger _logger;
        private readonly IProductFacade _productFacade;
        private readonly ISellerFacade _sellerFacade;
        private readonly ISaleConfigurationDbController _configurationDbController;
        private readonly IProductService _productService;

        public SaleConfigurationReader(IProductFacade productFacade, ISellerFacade sellerFacade, ISaleConfigurationDbController configurationDbController, ILogger logger, IProductService productService)
        {
            _productFacade = productFacade;
            _sellerFacade = sellerFacade;
            _configurationDbController = configurationDbController;
            _logger = logger;
            _productService = productService;
        }

        /// <summary>
        /// Get the Sales configuration by the Seller Name.
        /// WebShop id check has been removed internally for External Sales configuration
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="sellerName"></param>
        /// <param name="approved"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public SalesConfiguration GetSaleConfigurationForSeller(string isbn, string sellerName, bool approved, string version)
        {
            var webshopId = _sellerFacade.GetSellerWebShopId(sellerName);
            return GetSaleConfiguration(isbn, (WebShop)webshopId, approved, version);
        }
        public SalesConfiguration GetSaleConfiguration(string isbn, WebShop webShop, bool approved)
        {
            var state = approved ? Enums.EnumState.Approved : Enums.EnumState.Draft;
            var dbSaleConfig = _configurationDbController.GetSaleConfigurationForWebShop(isbn, webShop, state);
            return dbSaleConfig == null ? null : GetSalesConfig(dbSaleConfig);
        }
        public SalesConfiguration GetSaleConfiguration(string isbn, WebShop webShop, bool approved, string version)
        {
            var state = approved ? Enums.EnumState.Approved : Enums.EnumState.Draft;
            var dbSaleConfig = _configurationDbController.GetSaleConfigurationForWebShop(isbn, webShop, state);
            return dbSaleConfig == null ? null : GetSalesConfig(dbSaleConfig, version);
        }

        public SalesConfiguration GetSaleConfiguration(string isbn, WebShop webShop, bool approved, SalesConfigurationType salesConfigType)
        {
            var state = approved ? Enums.EnumState.Approved : Enums.EnumState.Draft;
            var dbSaleConfig = _configurationDbController.GetSaleConfiguration(isbn, state, webShop, salesConfigType);
            return dbSaleConfig == null ? null : GetSalesConfig(dbSaleConfig);
        }

        private SalesConfiguration GetSalesConfig(DBSaleConfiguration dbSaleConfiguration)
        {
            var salesConfig = new SalesConfiguration
            {
                // todo: Seller mapping of "Vidergaaende" is set to None to keep the change set minimum for hotfix
                Seller = dbSaleConfiguration.Seller.Id <= 4 ? (Api.ShopServices.Contracts.Enumerations.Seller)dbSaleConfiguration.Seller.Id : Api.ShopServices.Contracts.Enumerations.Seller.None,
                Isbn = dbSaleConfiguration.Isbn,
                Approved = dbSaleConfiguration.State == (int)Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumState.Approved,
                CreatedBy = dbSaleConfiguration.CreatedBy,
                CreatedDate = dbSaleConfiguration.CreatedDate,
                HasTrialAccess = dbSaleConfiguration.TrialLicense != null
            };

            salesConfig.TrialAccess = DbTrialLicenseToTrialLicense(dbSaleConfiguration);
            salesConfig.SalesForms = DbSaleFormToSaleForm(dbSaleConfiguration);
            salesConfig.AccessForms = GetAccessForm(dbSaleConfiguration);

            return salesConfig;
        }
        private SalesConfiguration GetSalesConfig(DBSaleConfiguration dbSaleConfiguration, string version)
        {
            var salesConfig = new SalesConfiguration
            {
                // todo: Seller mapping of "Vidergaaende" is set to None to keep the change set minimum for hotfix
                Seller = dbSaleConfiguration.Seller.Id <= 4 ? (Api.ShopServices.Contracts.Enumerations.Seller)dbSaleConfiguration.Seller.Id : Api.ShopServices.Contracts.Enumerations.Seller.None,
                Isbn = dbSaleConfiguration.Isbn,
                Approved = dbSaleConfiguration.State == (int)Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumState.Approved,
                CreatedBy = dbSaleConfiguration.CreatedBy,
                CreatedDate = dbSaleConfiguration.CreatedDate,
                HasTrialAccess = dbSaleConfiguration.TrialLicense != null
            };

            salesConfig.TrialAccess = DbTrialLicenseToTrialLicense(dbSaleConfiguration);
            salesConfig.SalesForms = DbSaleFormToSaleForm(dbSaleConfiguration);
            salesConfig.AccessForms = GetAccessForm(dbSaleConfiguration, version);

            return salesConfig;
        }
        private TrialAccess DbTrialLicenseToTrialLicense(DBSaleConfiguration dbSaleConfiguration)
        {
            if (dbSaleConfiguration.TrialLicense != null)
            {
                TrialAccess trialAccess = new TrialAccess()
                {
                    MultipleTrialsPerUser = dbSaleConfiguration.TrialLicense.MultipleTrials,
                    ContactSales = dbSaleConfiguration.TrialLicense.ContactSalesText
                };
                if (dbSaleConfiguration.TrialLicense.TrialAccessFormCode != null)
                {
                    trialAccess.AccessForm = new TrialAccessForm
                    {
                        Code = (EnumTrialAccessForm)dbSaleConfiguration.TrialLicense.RefTrialAccessForm.Code,
                        DisplayName = dbSaleConfiguration.TrialLicense.RefTrialAccessForm.DisplayName
                    };
                }
                if (dbSaleConfiguration.TrialLicense.TrialPeriodId != null)
                {
                    trialAccess.Period = new TrialPeriod
                    {
                        UnitValue = dbSaleConfiguration.TrialLicense.TrialPeriod.UnitValue,
                        UnitType = new TrialPeriodUnitType()
                        {
                            Code = (TrialPeriodUnitTypeCode)dbSaleConfiguration.TrialLicense.TrialPeriod.RefTrialPeriodTypeCode,
                            DisplayName = dbSaleConfiguration.TrialLicense.TrialPeriod.RefTrialPeriodUnitType.DisplayName
                        }
                    };
                }
                if (dbSaleConfiguration.TrialLicense.TrialCountId != null)
                {
                    trialAccess.TrialAccessCount = new TrialAccessCount()
                    {
                        UnitType = new TrialCountUnitType()
                        {
                            Code =
                                (TrialCountUnitTypeCode)dbSaleConfiguration.TrialLicense.TrialCount.RefCountUnitTypeCode,
                            DisplayName = dbSaleConfiguration.TrialLicense.TrialCount.RefTrialCountUnitType.DisplayName
                        },
                        UnitValue = dbSaleConfiguration.TrialLicense.TrialCount.UnitValue ?? 0
                    };
                }
                return trialAccess;
            }

            return null;
        }

        private List<SalesForm> DbSaleFormToSaleForm(DBSaleConfiguration dbSaleConfiguration)
        {
            if (dbSaleConfiguration.SalesFormId != null)
            {
                // setting sales configuration
                return new List<SalesForm>
                {
                    new SalesForm
                    {
                        Code = (EnumLicenseType) (dbSaleConfiguration.SalesForm?.RefSalesFormCode ?? 0),
                        DisplayName = dbSaleConfiguration.SalesForm?.RefSalesForm.DisplayName,
                    }
                };
            }

            return new List<SalesForm>();
        }

        private List<AccessForm> GetAccessForm(DBSaleConfiguration dbSaleConfiguration)
        {
            return dbSaleConfiguration.AccessForms.Select(af => new AccessForm
            {
                DisplayName = af.RefAccessForm.DisplayName,
                Code = (EnumAccessForm)af.RefCode,
                WebText = af.WebText,
                PriceModels =
                    af.PriceModels.Select(pm => new PriceModel
                    {
                        Code = (Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel)pm.RefPriceModelCode,

                        DisplayName = pm.RefPriceModel.DisplayName,
                        GradeLevels = GetGradesFromRapService(dbSaleConfiguration.Isbn, (Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel)pm.RefPriceModelCode),
                        PercentageValue = pm.PercentValue ?? 0,
                        ShowPercentage = pm.PercentValue != null,
                        RefAccessFormCode = pm.AccessForm.RefCode
                    }).ToList(),
                BillingPeriods = af.PeriodPrices.Select(pp => new Period
                {
                    DisplayName =
                        $"{pp.UnitValue} {pp.RefPeriodUnitType.DisplayName} {dbSaleConfiguration.SalesForm.RefSalesForm.PeriodTypeName}",
                    Currency = pp.Currency,
                    IsCustomPeriod = pp.IsCustomPeriod,
                    UnitValue = pp.UnitValue,
                    RefPeriodUnitTypeCode = (EnumPeriodUnitType)pp.RefPeriodUnitType.Code,
                    Price = new Price
                    {
                        VatValue = pp.VatValue,
                        UnitPrice = pp.UnitPrice,
                        UnitPriceVat = pp.UnitPriceVat
                    }
                }).OrderByDescending(bp => bp.RefPeriodUnitTypeCode).ThenBy(bp => bp.UnitValue).ToList(),
                DescriptionText = af.DescriptionText
            }).ToList();
        }
        private List<AccessForm> GetAccessForm(DBSaleConfiguration dbSaleConfiguration, string version)
        {
            return dbSaleConfiguration.AccessForms.Select(af => new AccessForm
            {
                DisplayName = af.RefAccessForm.DisplayName,
                Code = (EnumAccessForm)af.RefCode,
                WebText = af.WebText,
                PriceModels =
                    af.PriceModels.Select(pm => new PriceModel
                    {
                        Code = (Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel)pm.RefPriceModelCode,

                        DisplayName = pm.RefPriceModel.DisplayName,
                        GradeLevels = version == Constants.Version ? GetGradesFromRapService(dbSaleConfiguration.Isbn, (Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel)pm.RefPriceModelCode) : GetGrades(dbSaleConfiguration.Isbn, (Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel)pm.RefPriceModelCode),
                        PercentageValue = pm.PercentValue ?? 0,
                        ShowPercentage = pm.PercentValue != null,
                        RefAccessFormCode = pm.AccessForm.RefCode
                    }).ToList(),
                BillingPeriods = af.PeriodPrices.Select(pp => new Period
                {
                    DisplayName =
                        $"{pp.UnitValue} {pp.RefPeriodUnitType.DisplayName} {dbSaleConfiguration.SalesForm.RefSalesForm.PeriodTypeName}",
                    Currency = pp.Currency,
                    IsCustomPeriod = pp.IsCustomPeriod,
                    UnitValue = pp.UnitValue,
                    RefPeriodUnitTypeCode = (EnumPeriodUnitType)pp.RefPeriodUnitType.Code,
                    Price = new Price
                    {
                        VatValue = pp.VatValue,
                        UnitPrice = pp.UnitPrice,
                        UnitPriceVat = pp.UnitPriceVat
                    }
                }).OrderByDescending(bp => bp.RefPeriodUnitTypeCode).ThenBy(bp => bp.UnitValue).ToList(),
                DescriptionText = af.DescriptionText
            }).ToList();
        }
        private string GetGradesFromRapService(string isbn, Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel priceModel)
        {
            switch (priceModel)
            {
                case Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel.NoOfReleventClasses:
                case Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel.NoOfStudentsInReleventClasses:
                    try
                    {
                        var getGradeLevelsTask = Task.Run(() => _productFacade.GetGradeLevels(isbn));
                        getGradeLevelsTask.Wait();

                        var gradeLevelList = getGradeLevelsTask.Result;
                        if (!gradeLevelList.Any())
                            return "";

                        return ProcessGradeLevels.SortGradeLevels(gradeLevelList);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        throw new ProcessException((ulong)ErrorCodes.GradesNotFound, ErrorCodes.GradesNotFound.GetDescription(), Constants.OrigionatingSystemName);
                    }

                default:
                    return "";
            }
        }
        private string GetGrades(string isbn, Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel priceModel)
        {
            switch (priceModel)
            {
                case Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel.NoOfReleventClasses:
                case Api.ShopServices.Contracts.SalesConfiguration.Enums.EnumPriceModel.NoOfStudentsInReleventClasses:
                    try
                    {
                        var getGradeLevelsTask = Task.Run(() => _productService.GetGradeLevelsByIsbnAsync(isbn));
                        getGradeLevelsTask.Wait();

                        var gradeLevelList = getGradeLevelsTask.Result;
                        if (!gradeLevelList.Any())
                            return "";

                        return gradeLevelList;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                        throw new ProcessException((ulong)ErrorCodes.GradesNotFound, ErrorCodes.GradesNotFound.GetDescription(), Constants.OrigionatingSystemName);
                    }

                default:
                    return "";
            }
        }
    }
}