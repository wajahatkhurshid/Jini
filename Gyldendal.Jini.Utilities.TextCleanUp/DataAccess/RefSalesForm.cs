//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gyldendal.Jini.Utilities.TextCleanUp.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class RefSalesForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RefSalesForm()
        {
            this.SalesForms = new HashSet<SalesForm>();
        }
    
        public int Code { get; set; }
        public string DisplayName { get; set; }
        public int ExternalIdentifier { get; set; }
        public string PeriodTypeName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesForm> SalesForms { get; set; }
    }
}
