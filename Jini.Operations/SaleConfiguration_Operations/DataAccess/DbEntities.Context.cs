﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SalesConfigurationEntities : DbContext
    {
        public SalesConfigurationEntities()
            : base("name=SalesConfigurationEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__RefactorLog> C__RefactorLog { get; set; }
        public virtual DbSet<AccessForm> AccessForms { get; set; }
        public virtual DbSet<DbVersion> DbVersions { get; set; }
        public virtual DbSet<Dealer> Dealers { get; set; }
        public virtual DbSet<PeriodPrice> PeriodPrices { get; set; }
        public virtual DbSet<PriceModel> PriceModels { get; set; }
        public virtual DbSet<RefAccessForm> RefAccessForms { get; set; }
        public virtual DbSet<RefDealer> RefDealers { get; set; }
        public virtual DbSet<RefDealerText> RefDealerTexts { get; set; }
        public virtual DbSet<RefPeriod> RefPeriods { get; set; }
        public virtual DbSet<RefPeriodUnitType> RefPeriodUnitTypes { get; set; }
        public virtual DbSet<RefPriceModel> RefPriceModels { get; set; }
        public virtual DbSet<RefSalesChannel> RefSalesChannels { get; set; }
        public virtual DbSet<RefSalesConfigType> RefSalesConfigTypes { get; set; }
        public virtual DbSet<RefSalesForm> RefSalesForms { get; set; }
        public virtual DbSet<RefSeller> RefSellers { get; set; }
        public virtual DbSet<RefTrialAccessForm> RefTrialAccessForms { get; set; }
        public virtual DbSet<RefTrialCountUnitType> RefTrialCountUnitTypes { get; set; }
        public virtual DbSet<RefTrialPeriodUnitType> RefTrialPeriodUnitTypes { get; set; }
        public virtual DbSet<SalesConfiguration> SalesConfigurations { get; set; }
        public virtual DbSet<SalesConfigurationHistory> SalesConfigurationHistories { get; set; }
        public virtual DbSet<SalesForm> SalesForms { get; set; }
        public virtual DbSet<Seller> Sellers { get; set; }
        public virtual DbSet<TrialCount> TrialCounts { get; set; }
        public virtual DbSet<TrialLicense> TrialLicenses { get; set; }
        public virtual DbSet<TrialPeriod> TrialPeriods { get; set; }
    }
}