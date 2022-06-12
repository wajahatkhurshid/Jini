using Gyldendal.Jini.Services.Common.Dtos;

namespace Gyldendal.Jini.Services.Data.MappingHelper
{
    public static class DbMapper
    {
        public static DeflatedSalesConfigurationDto MapDeflatedSalesConfigurationToDto(this DeflatedSalesConfigurationView deflatedSalesConfigurationView)
        {
            return new DeflatedSalesConfigurationDto()
            {
                Id = deflatedSalesConfigurationView.Id,
                Isbn = deflatedSalesConfigurationView.Isbn,
                LastModified = deflatedSalesConfigurationView.CreatedDate,
                RefAccessFormCode = deflatedSalesConfigurationView.RefAccessFormCode,
                RefAccessFormDisplayName = deflatedSalesConfigurationView.RefAccessFormDisplayName,
                RefPeriodTypeCode = deflatedSalesConfigurationView.RefPeriodTypeCode,
                RefPeriodTypeDisplayName = deflatedSalesConfigurationView.RefPeriodTypeDisplayName,
                RefPriceModelCode = deflatedSalesConfigurationView.RefPriceModelCode,
                RefPriceModelDisplayName = deflatedSalesConfigurationView.RefPriceModelDisplayName,
                RefSalesCode = deflatedSalesConfigurationView.RefSalesCode,
                RefSalesDisplayName = deflatedSalesConfigurationView.RefSalesDisplayName,
                RefSalesConfigTypeCode = deflatedSalesConfigurationView.RefSalesConfigTypeCode,
                RevisionNumber = deflatedSalesConfigurationView.RevisionNumber,
                SalesChannel = deflatedSalesConfigurationView.SalesChannel,
                SalesFormId = deflatedSalesConfigurationView.SalesFormId,
                SellerId = deflatedSalesConfigurationView.SellerId,
                State = deflatedSalesConfigurationView.State,
                TrialLicenseId = deflatedSalesConfigurationView.TrialLicenseId,
                UnitPrice = deflatedSalesConfigurationView.UnitPrice,
                UnitPriceVat = deflatedSalesConfigurationView.UnitPriceVat,
                UnitValue = deflatedSalesConfigurationView.UnitValue,
                CreatedBy = deflatedSalesConfigurationView.CreatedBy,
                DepartmentCode = deflatedSalesConfigurationView.DepartmentCode
            };
        }
    }
}
