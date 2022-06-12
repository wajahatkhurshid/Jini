using System;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Common.Dtos
{
   public class ProductConfigWithAccessProvider
    {
        public int Id { get; set; }
        public string Isbn { get; set; }
        public int? SellerId { get; set; }
        public string SalesChannel { get; set; }
        public int? State { get; set; }
        public Nullable<int> SalesFormId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? RevisionNumber { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> TrialLicenseId { get; set; }
        public Nullable<int> RefSalesConfigTypeCode { get; set; }
        public List<AccessProvider> ProductAccessProviders { get; set; }
        public bool HasProductAccess { get; set; }
        public int? ProductAccessId { get; set; }

    }

    public class AccessProvider
    {
        public int Id { get; set; }
        public int AccessProviderId { get; set; }
    }
}
