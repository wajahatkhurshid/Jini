using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Contracts
{
    public class Products
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Isbn { get; set; }
        public string MediaTypeName { get; set; }
        public int? MediaTypeId { get; set; }
        public string MaterialTypeName { get; set; }
        public int? MaterialTypeId { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool HasProductAccess { get; set; }
        public int? State { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> TrialLicenseId { get; set; }
        public Nullable<int> RefSalesConfigTypeCode { get; set; }
        public int? ProductAccessId { get; set; }
        public int? SellerId { get; set; }
        public int? RevisionNumber { get; set; }
        public string SalesChannel { get; set; }
        public Nullable<int> SalesFormId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }

    }
}
