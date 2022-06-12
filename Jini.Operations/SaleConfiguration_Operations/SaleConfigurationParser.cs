using SaleConfiguration_Operations.DataAccess;
using System;
using System.Data;
using System.Linq;

namespace SaleConfiguration_Operations
{
    public class SaleConfigurationParser
    {
        public PeriodPrice Parse(SalesConfiguration salesConfiguration, DataRow sheetRow)
        {
            ParseSaleForm(salesConfiguration, sheetRow);

            var accessForm = ParseAccessForm(salesConfiguration, sheetRow);

            ParsePriceModel(accessForm, sheetRow);

            var billingPeriod = ParseBillingPeriod(accessForm, sheetRow);

            UpdatePrices(billingPeriod, sheetRow);

            return billingPeriod;
        }

        private void ParseSaleForm(SalesConfiguration salesConfiguration, DataRow sheetRow)
        {
            var saleFormCodeField = 3;
            if (salesConfiguration.SalesForm.RefSalesFormCode != int.Parse(sheetRow[saleFormCodeField].ToString()))
                throw new Exception($"Invalid sale form found for isbn: {sheetRow[0]} of type {sheetRow[saleFormCodeField - 1]} {sheetRow[saleFormCodeField]}");
        }

        private AccessForm ParseAccessForm(SalesConfiguration salesConfiguration, DataRow sheetRow)
        {
            var accessFormCodeField = 5;
            var accessForm = salesConfiguration.AccessForms.FirstOrDefault(z => z.RefCode == int.Parse(sheetRow[accessFormCodeField].ToString()));

            if (accessForm == null)
                throw new Exception($"No access form found for isbn: {sheetRow[0]} of type {sheetRow[accessFormCodeField - 1]} {sheetRow[accessFormCodeField]}");

            return accessForm;
        }

        private void ParsePriceModel(AccessForm accessForm, DataRow sheetRow)
        {
            if(accessForm.RefCode >= 1004)
                return;

            var priceModelCodeField = 7;

            if (!string.IsNullOrWhiteSpace(sheetRow[priceModelCodeField].ToString()))
            {
                var priceModel = accessForm.PriceModels.FirstOrDefault(pm => pm.RefPriceModelCode == int.Parse(sheetRow[priceModelCodeField].ToString()));
                if (priceModel == null)
                    throw new Exception($"No price model found for isbn: {sheetRow[0]} of access type {sheetRow[4]} {sheetRow[5]} " +
                                        $"and of price model {sheetRow[priceModelCodeField - 1]} {sheetRow[priceModelCodeField]}.");
            }
        }

        private PeriodPrice ParseBillingPeriod(AccessForm accessForm, DataRow sheetRow)
        {
            var unitValueField = 8;
            var refPeriodTypeCodeField = 10;

            var billingPeriod = accessForm.PeriodPrices.FirstOrDefault(x => x.UnitValue == int.Parse(sheetRow[unitValueField].ToString()) &&
                            x.RefPeriodTypeCode == int.Parse(sheetRow[refPeriodTypeCodeField].ToString()));

            if (billingPeriod == null)
                throw new Exception($"No billing period found for isbn: {sheetRow[0]} , of access type {sheetRow[4]} {sheetRow[5]}" +
                                    $" , of price model {sheetRow[6]} of billing period {sheetRow[unitValueField]} {sheetRow[refPeriodTypeCodeField]} .");

            return billingPeriod;
        }

        private void UpdatePrices(PeriodPrice billingPeriodPrice, DataRow sheetRow)
        {
            var newPriceField = 11;
            if (string.IsNullOrWhiteSpace(sheetRow[newPriceField].ToString()))
            {
                Logger.InfoLog($"No prices found for isbn: {sheetRow[0]} , of access type {sheetRow[4]} {sheetRow[5]}" +
                        $" , of price model {sheetRow[6]} of billing period {sheetRow[8]} {sheetRow[newPriceField]} .");
                return;
            }

            decimal newPrice;
            if (!decimal.TryParse(sheetRow[newPriceField].ToString(), out newPrice))
                throw new Exception($"Unable to parse the price for isbn: {sheetRow[0]} , of access type {sheetRow[4]} {sheetRow[5]}" +
                                    $" , of price model {sheetRow[6]} of billing period {sheetRow[8]} {sheetRow[newPriceField]} and the price value was {sheetRow[newPriceField]} .");

            billingPeriodPrice.UnitPrice = newPrice;
            billingPeriodPrice.UnitPriceVat = newPrice + ((newPrice / 100) * 25);
        }
    }
}