using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.ConfigurationManager;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Contracts;
using Gyldendal.Jini.Services.Core.Seller;
using Gyldendal.Jini.Services.Data;
using Gyldendal.Jini.Services.Data.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Common.Dtos;
using Enums = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.Enums;
using SalesConfiguration = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.SalesConfiguration;

namespace Gyldendal.Jini.Services.Core.Manager
{
    /// <summary>
    /// Jini Manager
    /// </summary>
    public class JiniManager : IJiniManager
    {
        private readonly ISellerFacade _sellerFacade;
        private ISaleConfigurationDbController _saleConfigurationDbController;

        public JiniManager(ISellerFacade sellerFacade, ISaleConfigurationDbController saleConfigurationDbController)
        {
            _sellerFacade = sellerFacade;
            _saleConfigurationDbController = saleConfigurationDbController;
        }

        /// <summary>
        /// Get Configuration of all products
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductConfigWithAccessProvider>> GetAllProductConfigWithAccessProvider()
        {
            return await _saleConfigurationDbController.GetAllSalesConfigurations();
        }

        ///  <summary>
        ///
        ///  </summary>
        ///  <param name="isbn"></param>
        public void UpdateConfigStatusOfCachedTitle(string isbn)
        {
            var result = MemoryCacher.GetValue(Common.Utils.Utils.RapProductsCacheToken);
            var digitalProduct = ((List<DigitalProduct>)result)?.FirstOrDefault(dt => dt.Isbn.Equals(isbn));
            if (digitalProduct != null)
            {
                digitalProduct.HasConfiguration = true;
                digitalProduct.ConfigurationText = new JiniConfigurationManager().ActiveConfiguration; //todo: change after Configuration Manager
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Data.SalesConfiguration> PopulateSalesConfiguration(SalesConfiguration configuration, int? refSalesConfigTypeCode = null)
        {
            var sellerId = _sellerFacade.GetSellerId(configuration.Seller.ToString());
            List<Data.SalesConfiguration> saleConfigs = new List<Data.SalesConfiguration>();

            var salesConfig = new Data.SalesConfiguration
            {
                Isbn = configuration.Isbn,
                CreatedDate = configuration.CreatedDate == DateTime.MinValue ? DateTime.Now : configuration.CreatedDate,
                SellerId = sellerId,
                CreatedBy = configuration.CreatedBy,
                SalesChannel = configuration.SalesChannel,
                RevisionNumber = 1,
                State = Enums.ToInt((configuration.Approved ? Enums.EnumState.Approved : Enums.EnumState.Draft)),
                RefSalesConfigTypeCode = refSalesConfigTypeCode
            };

            // if sale configuration has Trial Access
            if (configuration.HasTrialAccess)
            {
                var licenseTrailCount = configuration.TrialAccess.TrialAccessCount;

                // if user doesn't select any trial count
                if (!configuration.TrialAccess.MultipleTrialsPerUser)
                {
                    licenseTrailCount.UnitType = new TrialCountUnitType()
                    {
                        Code = TrialCountUnitTypeCode.Forever,
                    };
                    licenseTrailCount.UnitValue = int.MaxValue;
                }
                TrialCount trailCount = null;
                if (licenseTrailCount.UnitType != null)
                    trailCount = new TrialCount
                    {
                        UnitValue = licenseTrailCount.UnitValue,
                        RefCountUnitTypeCode = (int)licenseTrailCount.UnitType.Code
                    };

                Data.TrialPeriod trailPeriod = null;
                var licenseTrailPeriod = configuration.TrialAccess.Period;
                if (licenseTrailPeriod.UnitType != null)
                    trailPeriod = new Data.TrialPeriod
                    {
                        RefTrialPeriodTypeCode = (int)licenseTrailPeriod.UnitType.Code,
                        UnitValue = licenseTrailPeriod.UnitValue,
                    };

                var trialLicense = configuration.TrialAccess;
                var trailAccess = new Data.TrialLicense()
                {
                    TrialAccessFormCode = (int)trialLicense.AccessForm.Code,
                    ContactSalesText = HtmlPackUtil.RemoveHtmlExtras(trialLicense.ContactSales),
                    MultipleTrials = trialLicense.MultipleTrialsPerUser,
                };

                if (trailAccess.TrialAccessFormCode == 0)
                    trailAccess.TrialAccessFormCode = null;

                if (trailPeriod != null)
                    trailAccess.TrialPeriod = trailPeriod;
                if (trailCount != null)
                    trailAccess.TrialCount = trailCount;

                salesConfig.TrialLicense = trailAccess;
            }

            if (configuration.AccessForms.Any())
            {
                foreach (var af in configuration.AccessForms)
                {
                    var accessForm = new Data.AccessForm
                    {
                        RefCode = (int)af.Code,
                        WebText = af.WebText,
                    };
                    if (configuration.Approved && !af.PriceModels.Any() && af.Code != EnumAccessForm.SingleUser && af.Code != EnumAccessForm.ContactSales)
                    {
                        throw new Exception($"Provide PriceModels for accessForm:{af.Code.ToString()}.");
                    }
                    foreach (var pm in af.PriceModels.OrderBy(pm => pm.Code))
                    {
                        var priceModel = new Data.PriceModel
                        {
                            RefPriceModelCode = (int)pm.Code,
                            GradeLevels = pm.GradeLevels,
                            PercentValue = pm.PercentageValue
                        };
                        accessForm.PriceModels.Add(priceModel);
                    }
                    if (configuration.Approved && !af.BillingPeriods.Any() && af.Code != EnumAccessForm.ContactSales)
                    {
                        throw new Exception($"Provide BillingPeriods for accessForm:{af.Code.ToString()}.");
                    }
                    if (af.Code != EnumAccessForm.ContactSales)
                    {
                        foreach (var billingPeriod in af.BillingPeriods.OrderBy(bp => bp.RefPeriodUnitTypeCode))
                        {
                            var periodPrice = new PeriodPrice
                            {
                                Currency = billingPeriod.Currency,
                                IsCustomPeriod = billingPeriod.IsCustomPeriod || billingPeriod.Id == 0,
                                UnitPrice = billingPeriod.Price?.UnitPrice ?? 0.0m,
                                UnitPriceVat = billingPeriod.Price?.UnitPriceVat ?? 0.0m,
                                VatValue = billingPeriod.Price?.VatValue ?? 0,
                                RefPeriodTypeCode = (int)billingPeriod.RefPeriodUnitTypeCode,
                                UnitValue = billingPeriod.UnitValue
                            };
                            accessForm.PeriodPrices.Add(periodPrice);
                        }
                    }
                    else
                    {
                        accessForm.DescriptionText = HtmlPackUtil.RemoveHtmlExtras(af.DescriptionText);
                    }
                    salesConfig.AccessForms.Add(accessForm);
                }
            }

            if (configuration.SalesForms.Any() || (configuration.AccessForms.Count == 1 && configuration.AccessForms.First().Code == EnumAccessForm.ContactSales))
            {
                foreach (var salesForm in configuration.SalesForms)
                {
                    salesConfig.SalesForm = new Data.SalesForm()
                    {
                        RefSalesFormCode = (int)salesForm.Code
                    };
                }
                saleConfigs.Add(salesConfig);
            }
            else if (configuration.HasTrialAccess)
            {
                saleConfigs.Add(salesConfig);
            }
            else
            {
                throw new Exception("You can't create Sales Configuration");
            }
            return saleConfigs;
        }
    }
}