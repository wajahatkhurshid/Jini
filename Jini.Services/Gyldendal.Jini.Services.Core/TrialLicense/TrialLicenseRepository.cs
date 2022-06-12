using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Common.ConfigurationManager;
using Gyldendal.Jini.Services.Data;
using TrialAccess = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.TrialAccess;

namespace Gyldendal.Jini.Services.Core.TrialLicense
{
    public class TrialLicenseRepository : ITrialLicenseRepository
    {
        private readonly string _connectionString;

        public TrialLicenseRepository(IJiniConfigurationManager jiniConfigurationManager)
        {
            _connectionString = jiniConfigurationManager.ConnectionString;
        }

        /// <summary>
        /// Check whether Trail Access of an Isbn Exists or not
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="sellerName"></param>
        /// <returns></returns>
        public bool TrailAccessExists(string isbn, string sellerName)
        {
            bool exists = false;
            using (var db = new Jini_Entities(_connectionString))
            {
                var trailAccess =
                    db.SalesConfigurations.Include("Seller")
                        .FirstOrDefault(x => x.Isbn.Equals(isbn) && x.TrialLicenseId != null && x.Seller.Name.Equals(sellerName));
                if (trailAccess != null)
                {
                    exists = true;
                }
            }
            return exists;
        }

        public List<TrialAccess> GetAllTrailAccess()
        {
            using (var db = new Jini_Entities(_connectionString))
            {
                var trailAccess =
                    db.TrialLicenses.Include("RefTrialAccessForm").Select(x => new TrialAccess()
                    {
                        AccessForm = new TrialAccessForm
                        {
                            Code = (EnumTrialAccessForm)x.RefTrialAccessForm.Code,
                            DisplayName = x.RefTrialAccessForm.DisplayName
                        },
                        ContactSales = x.ContactSalesText,
                        MultipleTrialsPerUser = x.MultipleTrials,
                    }).ToList();

                return trailAccess;
            }
        }

        /// <summary>
        /// Update Trail License
        /// </summary>
        /// <param name="trialLicense"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        public bool UpdateTrialLicense(TrialAccess trialLicense, string seller)
        {
            try
            {
                using (var db = new Jini_Entities(_connectionString))
                {
                    var sellerId = db.Sellers.FirstOrDefault(x => x.Name.Equals(seller))?.Id;
                    if (!sellerId.HasValue)
                    {
                        //todo : handle error
                        throw new Exception("Invalid Seller");
                    }
                    var trailLicenseDb = new Data.TrialLicense();//db.TrialLicenses.Include("TrialCount").Include("TrialPeriod").FirstOrDefault(x => x.Isbn == trialLicense.);

                    // update Trail License properties
                    trailLicenseDb.TrialAccessFormCode = (int)trialLicense.AccessForm.Code;
                    trailLicenseDb.ContactSalesText = trialLicense.ContactSales;
                    trailLicenseDb.MultipleTrials = trialLicense.MultipleTrialsPerUser;

                    //Update Trail Count properties
                    trailLicenseDb.TrialCount.UnitValue = trialLicense.TrialAccessCount.UnitValue;
                    trailLicenseDb.TrialCount.RefCountUnitTypeCode = (int)trialLicense.TrialAccessCount.UnitType.Code;

                    //Update Trail Period properties
                    trailLicenseDb.TrialPeriod.RefTrialPeriodTypeCode = (int)trialLicense.Period.UnitType.Code;
                    trailLicenseDb.TrialPeriod.UnitValue = trialLicense.Period.UnitValue;

                    //Saving History

                    db.TrialLicenses.Attach(trailLicenseDb);
                    db.Entry(trailLicenseDb).State = EntityState.Modified;
                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates trail License
        /// </summary>
        /// <param name="trialLicense"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        public bool CreateTrialLicense(TrialAccess trialLicense, string seller)
        {
            try
            {
                if (!UpdateTrialLicense(trialLicense, seller))
                {
                    using (var db = new Jini_Entities(_connectionString))
                    {
                        var sellerId = db.Sellers.FirstOrDefault(x => x.Name.Equals(seller))?.Id;
                        if (!sellerId.HasValue)
                        {
                            //todo : handle error
                            throw new Exception("Invalid Seller");
                        }

                        var licenseTrailCount = trialLicense.TrialAccessCount;
                        var trailCount = new TrialCount()
                        {
                            UnitValue = licenseTrailCount.UnitValue,
                            RefCountUnitTypeCode = (int)licenseTrailCount.UnitType.Code
                        };

                        var licenseTrailPeriod = trialLicense.Period;
                        var trailPeriod = new Data.TrialPeriod()
                        {
                            RefTrialPeriodTypeCode = (int)licenseTrailPeriod.UnitType.Code,
                            UnitValue = licenseTrailPeriod.UnitValue
                        };

                        var trailAccess = new Data.TrialLicense()
                        {
                            TrialAccessFormCode = (int)trialLicense.AccessForm.Code,
                            ContactSalesText = trialLicense.ContactSales,
                            MultipleTrials = trialLicense.MultipleTrialsPerUser,
                            TrialCount = trailCount,
                            TrialPeriod = trailPeriod
                        };

                        db.TrialLicenses.Add(trailAccess);
                        db.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Get List of Trial AccessForms from RefTrialAccessForm table
        /// </summary>
        /// <returns>List of TrialAccessForm</returns>
        public List<TrialAccessForm> GetRefTrialAccessForms()
        {
            try
            {
                using (var db = new Jini_Entities(_connectionString))
                {
                    return db.RefTrialAccessForms.Select(x => new TrialAccessForm
                    {
                        Code = (EnumTrialAccessForm)x.Code,
                        DisplayName = x.DisplayName
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get Trail License
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        public TrialAccess GetTrialLicense(string isbn, string seller)
        {
            using (var db = new Jini_Entities(_connectionString))
            {
                var trialLicense = GetTrialLicenseFromDb(isbn, seller, db);
                if (trialLicense == null)
                    throw new Exception("Trial License Not Exists"); //todo : handle proper exception
                return TrialLicenseToTrialAccess(trialLicense);
            }
        }

        /// <summary>
        /// Get list of ref trial count unit types
        /// </summary>
        /// <returns>list of ref trial count unit types</returns>
        public List<TrialCountUnitType> GetRefTrialCountUnitTypes()
        {
            using (var db = new Jini_Entities(_connectionString))
            {
                return db.RefTrialCountUnitTypes.Select(x => new TrialCountUnitType
                {
                    Code = (TrialCountUnitTypeCode)x.Code,
                    DisplayName = x.DisplayName
                }).ToList();
            }
        }

        /// <summary>
        /// Get a list of PeriodsUnitTypes of provided salesForm
        /// </summary>
        /// <returns>returns list of PeriodsUnitTypes</returns>
        public List<TrialPeriodUnitType> GetRefTrialPeriodUnitTypes()
        {
            try
            {
                using (var db = new Jini_Entities(_connectionString))
                {
                    return db.RefTrialPeriodUnitTypes.Select(x => new TrialPeriodUnitType
                    {
                        Code = (TrialPeriodUnitTypeCode)x.Code,
                        DisplayName = x.DisplayName
                    }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove Trial License
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        public bool RemoveTrialLicense(string isbn, string seller = "Gyldendal Uddannelse")
        {
            using (var db = new Jini_Entities(_connectionString))
            {
                try
                {
                    var sellerId = db.Sellers.FirstOrDefault(x => x.Name == seller)?.Id;
                    if (sellerId == null)
                    {
                        //todo: handle exception
                        throw new Exception("Invalid Seller");
                    }
                    var trailLicenseDb = GetTrialLicenseFromDb(isbn, seller, db);
                    db.TrialCounts.Remove(trailLicenseDb.TrialCount);
                    db.TrialPeriods.Remove(trailLicenseDb.TrialPeriod);
                    db.TrialLicenses.Remove(trailLicenseDb);

                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    // todo : handle proper exception
                    Console.WriteLine(e);
                    return false;
                }
            }
        }

        /// <summary>
        /// Fetch Trial License From Database
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        private Data.TrialLicense GetTrialLicenseFromDb(string isbn, string seller, Jini_Entities db)
        {
            var salesConfiguration =
                db.SalesConfigurations
                .Include("TrialLicense")
                    .Include("TrialLicense.TrialPeriod")
                    .Include("TrialLicense.TrialCount")
                    .Include("TrialLicense.Seller")
                    .Include("TrialLicense.RefTrialAccessForm")
                    .FirstOrDefault(x => x.Isbn.Equals(isbn));
            return salesConfiguration?.TrialLicense;
        }

        /// <summary>
        /// Materialize Trial License to Contract
        /// </summary>
        /// <param name="trialLicense"></param>
        /// <returns></returns>
        private TrialAccess TrialLicenseToTrialAccess(Data.TrialLicense trialLicense)
        {
            return new TrialAccess
            {
                TrialAccessCount = new TrialAccessCount()
                {
                    UnitType = new TrialCountUnitType()
                    {
                        Code = (TrialCountUnitTypeCode)trialLicense.TrialCount.RefCountUnitTypeCode,
                        DisplayName = $"{trialLicense.TrialCount.UnitValue} {((TrialCountUnitTypeCode)trialLicense.TrialCount.RefCountUnitTypeCode).ToString()}"
                    },
                    UnitValue = trialLicense.TrialCount.UnitValue ?? 0
                },
                AccessForm = new TrialAccessForm
                {
                    Code = (EnumTrialAccessForm)trialLicense.RefTrialAccessForm.Code,
                    DisplayName = trialLicense.RefTrialAccessForm.DisplayName
                },
                MultipleTrialsPerUser = trialLicense.MultipleTrials,
                ContactSales = trialLicense.ContactSalesText,
                Period = new Api.ShopServices.Contracts.SalesConfiguration.TrialPeriod
                {
                    UnitValue = trialLicense.TrialPeriod.UnitValue,
                    UnitType = new TrialPeriodUnitType()
                    {
                        Code = (TrialPeriodUnitTypeCode)trialLicense.TrialPeriod.RefTrialPeriodTypeCode,
                        DisplayName = $"{trialLicense.TrialPeriod.UnitValue} {((TrialPeriodUnitTypeCode)trialLicense.TrialPeriod.RefTrialPeriodTypeCode).ToString()}"
                    },
                }
            };
        }
    }
}