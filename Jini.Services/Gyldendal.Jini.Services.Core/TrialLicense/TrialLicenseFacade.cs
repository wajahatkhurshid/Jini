using System.Collections.Generic;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.AccessServices.Contracts.LicenseModels;
using Gyldendal.Jini.Services.Common.ConfigurationManager;
using Gyldendal.Jini.Services.Core.Seller;

namespace Gyldendal.Jini.Services.Core.TrialLicense
{
    public class TrialLicenseFacade : ITrialLicenseFacade
    {
        private readonly ITrialLicenseRepository _trialLicenseRepository;
        private readonly ISellerFacade _sellerFacade;
        private readonly IJiniConfigurationManager _jiniConfigurationManager;
        public TrialLicenseFacade(ITrialLicenseRepository traiLicenseRepository, ISellerFacade sellerFacade, IJiniConfigurationManager jiniConfigurationManager)
        {
            _trialLicenseRepository = traiLicenseRepository;
            _sellerFacade = sellerFacade;
            _jiniConfigurationManager = jiniConfigurationManager;
        }

        public string GetTrialLicenseShareLink(string isbn, string seller)
        {
            var sellerId = _sellerFacade.GetSellerId(seller);
            var url = "";
            switch (sellerId)
            {
                case (int) Api.ShopServices.Contracts.Enumerations.Seller.GyldendalUddannelse:
                    url = _jiniConfigurationManager.GuBaseUrl;
                    break;
                case (int)Api.ShopServices.Contracts.Enumerations.Seller.HansReitzelsForlag:
                    url = _jiniConfigurationManager.HansReitzelUrl;
                    break;
                case (int)Api.ShopServices.Contracts.Enumerations.Seller.MunksgaardDanmark:
                    url = _jiniConfigurationManager.MunksGaardBaseUrl;
                    break;
            }

            url += $"?isbn={isbn}&seller={seller}";
            return url;
        }

        /// <summary>
        /// Returns List of Trial Access Forms from Jini Database
        /// </summary>
        /// <returns>Returns List of Trial Access Forms in Json</returns>
        public List<TrialAccessForm> GetRefTrialAccessForms()
        {
            return _trialLicenseRepository.GetRefTrialAccessForms();
        }

        /// <summary>
        /// Get a list of PeriodsUnitTypes of provided salesForm
        /// </summary>
        /// <returns>returns list of PeriodsUnitTypes</returns>
        public List<TrialPeriodUnitType> GetRefTrialPeriodUnitTypes()
        {
            return _trialLicenseRepository.GetRefTrialPeriodUnitTypes();
        }

        /// <summary>
        /// Get list of ref trial count unit types
        /// </summary>
        /// <returns>list of ref trial count unit types</returns>
        public List<TrialCountUnitType> GetRefTrialCountUnitTypes()
        {
            return _trialLicenseRepository.GetRefTrialCountUnitTypes();
        }
    }
}