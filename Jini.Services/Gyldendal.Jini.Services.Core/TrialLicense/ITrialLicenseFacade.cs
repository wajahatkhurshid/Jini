using System.Collections.Generic;
using Gyldendal.AccessServices.Contracts.LicenseModels;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;

namespace Gyldendal.Jini.Services.Core.TrialLicense
{
    public interface ITrialLicenseFacade
    {
        /// <summary>
        /// Create and return a shareable url to shops
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
        string GetTrialLicenseShareLink(string isbn, string seller);

        /// <summary>
        /// Returns List of Trial Access Forms from Jini Database
        /// </summary>
        /// <returns>Returns List of Trial Access Forms in Json</returns>
        List<TrialAccessForm> GetRefTrialAccessForms();

        /// <summary>
        /// Get a list of PeriodsUnitTypes of provided salesForm
        /// </summary>
        /// <returns>returns list of PeriodsUnitTypes</returns>
        List<TrialPeriodUnitType> GetRefTrialPeriodUnitTypes();

        /// <summary>
        /// Get list of ref trial count unit types
        /// </summary>
        /// <returns>list of ref trial count unit types</returns>
        List<TrialCountUnitType> GetRefTrialCountUnitTypes();
        
    }
}