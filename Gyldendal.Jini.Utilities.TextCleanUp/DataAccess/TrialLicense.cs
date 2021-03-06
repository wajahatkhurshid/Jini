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
    
    public partial class TrialLicense
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrialLicense()
        {
            this.SalesConfigurations = new HashSet<SalesConfiguration>();
        }
    
        public int Id { get; set; }
        public Nullable<int> TrialPeriodId { get; set; }
        public bool MultipleTrials { get; set; }
        public Nullable<int> TrialAccessFormCode { get; set; }
        public Nullable<int> TrialCountId { get; set; }
        public string ContactSalesText { get; set; }
    
        public virtual RefTrialAccessForm RefTrialAccessForm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SalesConfiguration> SalesConfigurations { get; set; }
        public virtual TrialCount TrialCount { get; set; }
        public virtual TrialPeriod TrialPeriod { get; set; }
    }
}
