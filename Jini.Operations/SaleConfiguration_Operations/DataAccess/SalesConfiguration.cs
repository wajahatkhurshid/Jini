//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SaleConfiguration_Operations.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class SalesConfiguration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SalesConfiguration()
        {
            this.AccessForms = new HashSet<AccessForm>();
        }
    
        public int Id { get; set; }
        public string Isbn { get; set; }
        public int SellerId { get; set; }
        public string SalesChannel { get; set; }
        public int State { get; set; }
        public Nullable<int> SalesFormId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int RevisionNumber { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> TrialLicenseId { get; set; }
        public Nullable<int> RefSalesConfigTypeCode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccessForm> AccessForms { get; set; }
        public virtual RefSalesConfigType RefSalesConfigType { get; set; }
        public virtual SalesForm SalesForm { get; set; }
        public virtual Seller Seller { get; set; }
        public virtual TrialLicense TrialLicense { get; set; }
    }
}
