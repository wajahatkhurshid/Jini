//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gyldendal.Jini.Services.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeflatedSalesConfigurationView
    {
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
        public Nullable<int> RefSalesCode { get; set; }
        public string RefSalesDisplayName { get; set; }
        public Nullable<int> RefAccessFormCode { get; set; }
        public string RefAccessFormDisplayName { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> UnitPriceVat { get; set; }
        public Nullable<int> UnitValue { get; set; }
        public Nullable<int> RefPeriodTypeCode { get; set; }
        public string RefPeriodTypeDisplayName { get; set; }
        public Nullable<int> RefPriceModelCode { get; set; }
        public string RefPriceModelDisplayName { get; set; }
        public Nullable<long> Id { get; set; }
        public Nullable<int> IsExternalLogin { get; set; }
        public int ProductAccessProvider { get; set; }
        public string DepartmentCode { get; set; }
    }
}