//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gyldendal.Jini.SalesConfigurationServices.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class PeriodPrice
    {
        public int Id { get; set; }
        public int UnitValue { get; set; }
        public int RefPeriodTypeCode { get; set; }
        public int AccessFormId { get; set; }
        public string Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceVat { get; set; }
        public int VatValue { get; set; }
        public bool IsCustomPeriod { get; set; }
    
        public virtual AccessForm AccessForm { get; set; }
        public virtual RefPeriodUnitType RefPeriodUnitType { get; set; }
    }
}