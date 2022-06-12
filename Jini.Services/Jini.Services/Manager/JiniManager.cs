using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Jini.Services.Properties;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Data;
using Enums = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.Enums;
using SalesConfiguration = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.SalesConfiguration;

namespace Gyldendal.Jini.Services.Manager
{
    /// <summary>
    /// Jini Manager
    /// </summary>
    public class JiniManager : IJiniManager
    {
        /// <summary>
        /// Get Configuration of all products
        /// </summary>
        /// <returns></returns>
        public List<Data.SalesConfiguration> GetAllConfigurations()
        {
            return DbController.GetAllSalesConfigurations();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        public void UpdateConfigStatusOfCachedTitle(string isbn, string seller = "")
        {
            var result = MemoryCacher.GetValue(Common.Utils.Utils.RapProductsCacheToken);
            var digitalProduct = ((List<DigitalProduct>)result)?.FirstOrDefault(dt => dt.Isbn.Equals(isbn));
            if (digitalProduct != null)
            {
                digitalProduct.HasConfiguration = true;
                digitalProduct.ConfigurationText = Settings.Default.ActiveConfiguration;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Data.SalesConfiguration> PopulateSalesConfiguration(SalesConfiguration configuration)
        {
            var seller = DbController.GetSellerByName(configuration.Seller);
            if (seller == null)
            {
                throw new Exception("Seller not found.");
            }
            List<Data.SalesConfiguration> saleConfigs = new List<Data.SalesConfiguration>();
            if (configuration.Approved && !configuration.SalesForms.Any())
            {
                throw new Exception("Provide Sales Forms.");
            }
            foreach (var salesForm in configuration.SalesForms)
            {
                var salesConfig = new Data.SalesConfiguration
                {
                    Isbn = configuration.Isbn,
                    CreatedDate = DateTime.Now,
                    SellerId = seller.Id,
                    SalesForm = new Data.SalesForm
                    {
                        RefSalesFormCode = (int)salesForm.Code,
                    },
                    CreatedBy = configuration.CreatedBy,
                    SalesChannel = configuration.SalesChannel,
                    RevisionNumber = 1,
                    State = Enums.ToInt((configuration.Approved ? Enums.EnumState.Approved : Enums.EnumState.Draft))
                };
                if (configuration.Approved && !configuration.AccessForms.Any())
                {
                    throw new Exception($"Provide Access Form for SalesForm:{salesForm.Code.ToString()}.");
                }
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
                        accessForm.DescriptionText = af.DescriptionText;
                    }
                    salesConfig.AccessForms.Add(accessForm);
                }
                saleConfigs.Add(salesConfig);
            }

            return saleConfigs;
        }
    }
}