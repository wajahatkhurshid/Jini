using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Jini.Services.Core.Manager;
using Gyldendal.Jini.Services.Core.SaleConfiguration;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Filters;
using Gyldendal.Jini.Services.Utils;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using SalesConfiguration = Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.SalesConfiguration;

namespace Gyldendal.Jini.Services.Controllers
{
    /// <summary>
    /// Sales Configuration Controller
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SalesConfigurationController : ApiController
    {
        private readonly ILogger _logger;

        private ISaleConfigurationFacade _saleConfigurationFacade;

        private IJiniManager _jiniManager;

        //todo: remove DAL object dependency from controller
        private ISaleConfigurationDbController _saleConfigurationDbController;

        /// <summary>
        ///
        /// </summary>
        /// <param name="saleConfigurationFacade"></param>
        /// <param name="jiniManager"></param>
        /// <param name="saleConfigurationDbController"></param>
        public SalesConfigurationController(ISaleConfigurationFacade saleConfigurationFacade,
            IJiniManager jiniManager, ISaleConfigurationDbController saleConfigurationDbController, ILogger logger)
        {
            _saleConfigurationFacade = saleConfigurationFacade;
            _jiniManager = jiniManager;
            _saleConfigurationDbController = saleConfigurationDbController;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of Period Types
        /// </summary>
        /// <returns>returns list of Periods</returns>
        [Route("api/v1/SalesConfiguration/Save/")]
        [NullValueFilter]
        [HttpPost]
        public IHttpActionResult CreateSalesConfiguration(SalesConfiguration configuration)
        {
            _logger.LogInfo(_logger.GetPropertyNameAndValue(() => configuration), true, isGdprSafe: false);
            try
            {
                var salesConfigurations = _saleConfigurationDbController.GetSalesConfigurations(configuration.Isbn, configuration.Seller);
                var newSalesConfigurations = _jiniManager.PopulateSalesConfiguration(configuration);

                // No Sales configuration Exists
                bool result;

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (salesConfigurations.Count == 0)
                {
                    result = _saleConfigurationDbController.CreateSalesConfiguration(newSalesConfigurations.FirstOrDefault());
                }

                // If sales configuration Exists then Update
                else
                {
                    result = _saleConfigurationDbController.UpdateSalesConfiguration(salesConfigurations, newSalesConfigurations.FirstOrDefault());
                }
                if (result)
                {
                    if (configuration.Approved)
                    {
                        _jiniManager.UpdateConfigStatusOfCachedTitle(configuration.Isbn);
                    }
                    return Ok();
                }
                return BadRequest("Unable to Create Sales Configuration, Try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex, isGdprSafe: false);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retreive sales configuration hisotry for a particular ISBN.
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v1/SalesConfigurationHistory/Get/{isbn}")]
        [HttpGet]
        public IHttpActionResult SalesConfigurationHistory(string isbn)
        {
            try
            {
                return Ok(_saleConfigurationDbController.GetConfigurationVersionsHistory(isbn));
            }
            catch (Exception ex)
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => isbn), true, isGdprSafe: false);
                _logger.LogError(ex.Message, ex, isGdprSafe: false);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Check if Sales configuration Exists of ISBN for Particular Seller.
        /// Seller Name check has been removed internally for External Sales configuration
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v1/SalesConfiguration/Get/{isbn}/{seller}/{version}/{approved}")]
        [HttpGet]
        public IHttpActionResult GetSalesConfiguration(string isbn, string seller, string version, bool approved = true)
        {
            return Ok(_saleConfigurationFacade.GetSaleConfiguration(isbn, seller, approved, version));
        }

        /// <summary>
        /// Get Sales configuration for webshop
        /// WebShop id check has been removed internally for External Sales configuration
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="webShop"></param>
        /// <param name="approved"></param>
        /// <returns></returns>
        [Route("api/v1/SalesConfiguration/Get/WebShop/{isbn}/{webShop}/{approved}")]
        [HttpGet]
        public IHttpActionResult GetSalesConfigurationByWebShop(string isbn, WebShop webShop ,bool approved = true)
        {
            return Ok(_saleConfigurationFacade.GetSaleConfiguration(isbn, webShop, approved));
        }

        /// <summary>
        /// Get Internal/External Sales configuration for webshop
        /// WebShop id check has been removed internally for External Sales configuration
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="webShop"></param>
        /// <param name="approved"></param>
        /// <param name="salesConfigType"></param>
        /// <returns></returns>
        [Route("api/v1/SalesConfiguration/Get/WebShop/{isbn}/{webShop}/{salesConfigType}/{approved}")]
        [HttpGet]
        public IHttpActionResult GetSalesConfigurationByWebShop(string isbn, WebShop webShop, SalesConfigurationType salesConfigType,bool approved = true)
        {
            return Json(_saleConfigurationFacade.GetSaleConfiguration(isbn, webShop, approved, salesConfigType));
        }

        /// <summary>
        /// Check if Sales configuration Exists of ISBN for Particular Seller.
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v1/SalesConfiguration/Exists/{isbn}/{seller}")]
        [HttpGet]
        public IHttpActionResult SalesConfigurationExists(string isbn, string seller)
        {
            return Ok(_saleConfigurationDbController.SalesConfigurationExistBySeller(isbn, seller));
        }

        /// <summary>
        /// Check if Sales configuration Exists of ISBN for Particular Seller.
        /// WebShop id check has been removed internally for External Sales configuration
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v1/SalesConfiguration/Exists/webshop/{isbn}/{webShop}")]
        [HttpGet]
        public IHttpActionResult SalesConfigurationExistsByWebShop(string isbn, WebShop webShop)
        {
            return Ok(_saleConfigurationDbController.SalesConfigurationExistByWebShop(isbn, webShop));
        }

        /// <summary>
        /// Get details for the Sales Configurations updated after the given time for the given seller.
        /// WebShop id check has been removed internally for External Sales configuration
        /// </summary>
        [Route("api/v1/SalesConfiguration/GetUpdatedSaleConfigsInfo/{updatedAfterTicks}/{webShop}/{take}")]
        [HttpGet]
        public IHttpActionResult GetUpdatedSaleConfigsInfo(long updatedAfterTicks, WebShop webShop, int take)
        {
            var updatedAfter = new DateTime(updatedAfterTicks);

            return Ok(_saleConfigurationDbController.GetUpdatedSaleConfigsInfo(updatedAfter, webShop, take));
        }

        // Unused actions - In future move their logic to backend and uncomment if required
        ///// <summary>
        ///// Gets all sales configurations of comma separated isbns and selerializes them in a json file
        ///// </summary>
        ///// <returns>returns nothing but status ok</returns>
        //[Route("api/v1/SalesConfiguration/SerializeConfigurationsIntoTheFile/")]
        //[NullValueFilter]
        //[HttpPost]
        //public IHttpActionResult SerializeConfigurationsIntoTheFile(string isbnsToSerialize)
        //{
        //    try
        //    {
        //        Logger.LogInfo("Serialization Statred", true);
        //        List<SalesConfiguration> configurations = new List<SalesConfiguration>();
        //        foreach (string isbn in isbnsToSerialize.Split(','))
        //        {
        //            Logger.LogInfo("Adding isbn " + isbn + " in the configurations list object ", true);

        //            var salesConfigs = DbController.GetSalesConfiguration(isbn.Trim()).OrderByDescending(x => x.RevisionNumber).ToList();

        //            foreach (var salesConfig in salesConfigs)
        //            {
        //                if (salesConfig != null && salesConfig.State == (int)EnumState.Approved)
        //                {
        //                    configurations.Add(DbController.DbSaleConfigToShopServiceSaleConfig(salesConfig));
        //                }
        //            }

        //        }

        //        if (configurations.Count > 0)
        //        {
        //            if (File.Exists(HttpContext.Current.Server.MapPath(@"~/App_Data/configurations.json")))
        //            {
        //                File.Delete(HttpContext.Current.Server.MapPath(@"~/App_Data/configurations.json"));
        //            }
        //            System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath(@"~/App_Data/configurations.json"), JsonConvert.SerializeObject(configurations));
        //            Logger.LogInfo("written all sales configuration in the add data json file configurations.json ", true);

        //            return Ok();
        //        }
        //        else
        //        {
        //            return BadRequest("Unable to Create Sales Configuration, Try again.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex.Message, ex);
        //        return InternalServerError(ex);
        //    }
        //}

        ///// <summary>
        ///// Gets all sales configurations of comma separated isbns and selerializes them in a json file
        ///// </summary>
        ///// <returns>returns nothing but status ok</returns>
        //[Route("api/v1/SalesConfiguration/DeserializeSalesConfigsFromFileintoDB/")]
        //[NullValueFilter]
        //[HttpPost]
        //public IHttpActionResult DeserializeSalesConfigsFromFileintoDB()
        //{
        //    try
        //    {
        //        Logger.LogInfo("Deserialization Started ", true);
        //        if (File.Exists(HttpContext.Current.Server.MapPath(@"~/App_Data/configurations.json")))
        //        {
        //            List<SalesConfiguration> configurations = JsonConvert.DeserializeObject<List<SalesConfiguration>>(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/App_Data/configurations.json")));

        //            if (configurations.Count > 0)
        //            {
        //                File.Delete(HttpContext.Current.Server.MapPath(@"~/App_Data/configurations.json"));
        //                DbController.DeleteSalesConfiguration(configurations.Select(x => x.Isbn).Distinct().ToArray());
        //                Logger.LogInfo("Deleted existing configurations against the selected isbns", true);
        //            }

        //            foreach (SalesConfiguration configuration in configurations)
        //            {
        //                Logger.LogInfo("adding old configuration for the isbn " + configuration.Isbn, true);

        //                //configuration.Seller = configuration.Seller == Seller.Nonfiktion
        //                //    ? Seller.GyldendalUddannelse
        //                //    : configuration.Seller;
        //                var salesConfigurations = DbController.GetSalesConfigurations(configuration.Isbn, configuration.Seller);
        //                var newSalesConfigurations = new JiniManager().PopulateSalesConfiguration(configuration);

        //                // No Sales configuration Exists
        //                bool result;

        //                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        //                if (salesConfigurations.Count == 0)
        //                {
        //                    result = DbController.CreateSalesConfiguration(newSalesConfigurations.FirstOrDefault());
        //                }

        //                // If sales configuration Exists then Update
        //                else
        //                {
        //                    result = DbController.UpdateSalesConfiguration(salesConfigurations, newSalesConfigurations.FirstOrDefault());
        //                }
        //                if (result)
        //                {
        //                    if (configuration.Approved)
        //                    {
        //                        var jiniManager = new JiniManager();
        //                        jiniManager.UpdateConfigStatusOfCachedTitle(configuration.Isbn);
        //                    }

        //                }
        //            }
        //        }
        //        Logger.LogInfo("Deserialization Done ", true);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex.Message, ex);
        //        return InternalServerError(ex);
        //    }
        //}

        /// <summary>
        /// Retreive sales configuration Revisionhisotry for a particular ISBN.
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v1/SalesConfigurationRevisionHistory/Get/{isbn}")]
        [HttpGet]
        public IHttpActionResult SalesConfigurationRevisionHistory(string isbn)
        {
            try
            {
                return Ok(_saleConfigurationDbController.GetConfigurationVersionsRevisionHistory(isbn));
            }
            catch (Exception ex)
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => isbn), true, isGdprSafe: false);
                _logger.LogError(ex.Message, ex, isGdprSafe: false);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retreive sales configuration Revisionhisotry for a particular ISBN (GUA).
        /// </summary>
        /// <returns>returns true/false</returns>
        [Route("api/v1/GuaSalesConfigurationRevisionHistory/Get/{isbn}")]
        [HttpGet]
        public IHttpActionResult GuaSalesConfigurationRevisionHistory(string isbn)
        {
            try
            {
                return Ok(_saleConfigurationDbController.GetGUAConfigurationVersionsRevisionHistory(isbn));
            }
            catch (Exception ex)
            {
                _logger.LogInfo(_logger.GetPropertyNameAndValue(() => isbn), true, isGdprSafe: false);
                _logger.LogError(ex.Message, ex, isGdprSafe: false);
                return InternalServerError(ex);
            }
        }
    }
}