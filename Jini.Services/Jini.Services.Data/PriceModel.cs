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
    
    public partial class PriceModel
    {
        public int id { get; set; }
        public int AccessFormId { get; set; }
        public int RefPriceModelCode { get; set; }
        public Nullable<int> PercentValue { get; set; }
        public string GradeLevels { get; set; }
    
        public virtual AccessForm AccessForm { get; set; }
        public virtual RefPriceModel RefPriceModel { get; set; }
    }
}
