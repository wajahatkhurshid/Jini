using System.Collections.Generic;
using Gyldendal.AccessServices.Contracts.LicenseModels;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using TrialAccess = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.TrialAccess;

namespace Gyldendal.Jini.Services.Core.TrialLicense
{
    public interface ITrialLicenseRepository
    {
        bool TrailAccessExists(string isbn, string sellerName);

        List<TrialAccess> GetAllTrailAccess();

        /// <summary>
        /// Trail License updation
        /// </summary>
        /// <param name="trialLicense"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        bool UpdateTrialLicense(TrialAccess trialLicense, string seller);

        bool CreateTrialLicense(TrialAccess trialLicense, string seller);

        /// <summary>
        /// Get List of Trial AccessForms from RefTrialAccessForm table
        /// </summary>
        /// <returns>List of TrialAccessForm</returns>
        List<TrialAccessForm> GetRefTrialAccessForms();

        TrialAccess GetTrialLicense(string isbn, string seller);

        /// <summary>
        /// Get list of ref trial count unit types
        /// </summary>
        /// <returns>list of ref trial count unit types</returns>
        List<TrialCountUnitType> GetRefTrialCountUnitTypes();

        /// <summary>
        /// Get a list of PeriodsUnitTypes of provided salesForm
        /// </summary>
        /// <returns>returns list of PeriodsUnitTypes</returns>
        List<TrialPeriodUnitType> GetRefTrialPeriodUnitTypes();

        /// <summary>
        /// Remove Trail License from Db
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        bool RemoveTrialLicense(string isbn, string seller = "Gyldendal Uddannelse");
    }
}