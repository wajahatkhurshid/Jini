using Gyldendal.Jini.Services.Common.Dtos;
using Gyldendal.Jini.Services.Data.MappingHelper.RevsionHistory;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Jini.Services.Common;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;

namespace Gyldendal.Jini.Services.Data.MappingHelper
{
    public static class Mapper
    {
       
        public static SalesConfigurationRevisionHistoryDto MapSalesConfigurationRevisionHistoryDt(this SalesConfigurationHistory salesConfigurationHistory)
        {
            var result= new SalesConfigurationRevisionHistoryDto()
            {
                Id = salesConfigurationHistory.Id,
                CreatedBy = salesConfigurationHistory.CreatedBy,
                CreatedDate = salesConfigurationHistory.CreatedDate,
                Isbn = salesConfigurationHistory.Isbn,
                VersionNo = salesConfigurationHistory.VersionNo,
                SalesConfiguration = JsonConvert.DeserializeObject<SalesConfiguration>(salesConfigurationHistory.Value, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                })
            };

            result.RevisionHistoryData = MapRevisionHistoryDt(result.SalesConfiguration);
            return result;
        }

        public static List<RevisionHistoryData> MapRevisionHistoryDt(SalesConfiguration salesConfiguration)
        {
            var result = new List<RevisionHistoryData>();
            foreach (var item in salesConfiguration.AccessForms)
            {
                if (item.PeriodPrices.Any())
                {
                    foreach (var periodPrice in item.PeriodPrices)
                    {
                        var historyData = new RevisionHistoryData();
                        var refPeriodText = TrailUnitText(periodPrice.RefPeriodTypeCode);
                        historyData.SubscriptionText = salesConfiguration.SalesForm?.RefSalesFormCode == 1001 ? Constants.SalesFormDisplayNameForSubscription : Constants.SalesFormDisplayNameForRental;
                        historyData.SalgsformText = GenerateSalesFormText(salesConfiguration, item.RefCode);
                        historyData.Price = periodPrice.UnitPrice;
                        historyData.PriceVat = periodPrice.UnitPriceVat;
                        historyData.PeriodText = periodPrice.UnitValue + " " + refPeriodText;
                        result.Add(historyData);
                    }
                } 
            }
            if (salesConfiguration.TrialLicense != null)
            {
                var historyData = new RevisionHistoryData();
                historyData.SubscriptionText = string.Empty;
                historyData.SalgsformText = Constants.TrialLicenseDisplayName;
                historyData.PeriodText = salesConfiguration.TrialLicense.TrialPeriod.UnitValue + " " + TrailUnitText(salesConfiguration.TrialLicense.TrialPeriod.RefTrialPeriodTypeCode);
                historyData.MaxAntalText = salesConfiguration.TrialLicense.MultipleTrials == false ? Constants.RenderNo : Constants.RenderYes + "," + salesConfiguration.TrialLicense.TrialCount.UnitValue + " " + TrailPeriodText(salesConfiguration.TrialLicense.TrialCount.RefCountUnitTypeCode);
                result.Add(historyData);
            }
            foreach (var item in salesConfiguration.AccessForms)
            {
                if (!item.PeriodPrices.Any())
                {
                        var historyData = new RevisionHistoryData();
                        historyData.SalgsformText = SalgsformText(item.RefCode);
                        historyData.MaxAntalText = item.DescriptionText;
                        result.Add(historyData);
                }
            }
           
            return result;
        }

        public static string TrailPeriodText(int code)
        {
            if (code == 0)
            {
                return string.Empty;
            }
            Dictionary<int, string> trails = new Dictionary<int, string>()
            {
                { 1001, "per år" },
                { 1002, "per måned" },
                { 1003, "for altid" }
            };

            return trails[code];
        }


        public static string TrailUnitText(int code)
        {
            if (code == 0)
            {
                return string.Empty;
            }
            Dictionary<int, string> trails = new Dictionary<int, string>()
            {  
                { 1001,  "år" },
                { 1002, "måneders" },
                { 1003, "dages" },
                { 1004, "timers" }
            };

            return trails[code];
        }
        public static string SalgsformText(int code)
        {
            if (code == 0)
            {
                return string.Empty;
            }
            Dictionary<int, string> salgs = new Dictionary<int, string>()
            {
                 { 1001, "Skolelicens" },
                 { 1002, "Klasselicens" },
                 { 1003, "Underviserlicens" },
                 { 1004, "Enkeltbrugerlicens" },
                 { 1005, "Friteksfelt" },
            };

            return salgs[code]; 
        }

        public static string PriceModelText(int code)
        {
            if (code == 0)
            {
                return string.Empty;
            }
            Dictionary<int, string> refPriceModel = new Dictionary<int, string>()
            {
                 { 1001, "Hele skolen (enhedspris)" },
                 { 1002, "Antal elever" },
                 { 1003, "Procentdel af elever" },
                 { 1004, "Antal elever på relevante klassetrin" },
                 { 1005, "Antal klasser på relevante klassetrin" },
                 { 1006, "Antal klasser" },
                 { 1007, "Antal elever i klasser" },
                 { 1008, "Hele skolen (enhedspris)" },
            };

            return refPriceModel[code];
        }

        private static string GenerateSalesFormText(SalesConfiguration salesObj, int refCode)
        {
            var salesFormText = string.Empty;

            if (salesObj.RefSalesConfigTypeCode == (int)SalesConfigurationType.Internal)
            {
                salesFormText = "GUA";
            }
            else
            {
             salesFormText =   SalgsformText(refCode) + " / " + PriceModelText(salesObj.AccessForms?.FirstOrDefault()?.PriceModels.FirstOrDefault()?.RefPriceModelCode ?? 0);
            }

            return salesFormText;
        }
    }
}
