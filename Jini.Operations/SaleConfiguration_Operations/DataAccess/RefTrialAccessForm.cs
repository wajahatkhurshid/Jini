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
    
    public partial class RefTrialAccessForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RefTrialAccessForm()
        {
            this.TrialLicenses = new HashSet<TrialLicense>();
        }
    
        public int Code { get; set; }
        public string DisplayName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrialLicense> TrialLicenses { get; set; }
    }
}
