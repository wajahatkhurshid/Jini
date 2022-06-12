using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Common.Dtos
{
    public class DeflatedSalesConfigurationV2Dto
    {
        public long? Id { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public int SellerId { get; set; }
        public string SalesChannel { get; set; }
        public int State { get; set; }
        public int? SalesFormId { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? LastModified { get; set; }
        public int RevisionNumber { get; set; }
        public string CreatedBy { get; set; }
        public int? TrialLicenseId { get; set; }
        public int? RefSalesConfigTypeCode { get; set; }
        public int? RefSalesCode { get; set; }
        public string RefSalesDisplayName { get; set; }
        public int? RefAccessFormCode { get; set; }
        public string RefAccessFormDisplayName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitPriceVat { get; set; }
        public int? UnitValue { get; set; }
        public int? RefPeriodTypeCode { get; set; }
        public string RefPeriodTypeDisplayName { get; set; }
        public int? RefPriceModelCode { get; set; }
        public string RefPriceModelDisplayName { get; set; }
        public int? GpmMediaTypeId { get; set; }
        public string GpmMaterialTypeId { get; set; }
        public string DepartmentCode { get; set; }
        public int? IsExternalLogin { get; set; }
        public int ProductAccessProvider { get; set; }
    }
}
