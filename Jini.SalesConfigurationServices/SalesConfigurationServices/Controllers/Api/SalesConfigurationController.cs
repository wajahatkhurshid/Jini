using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using Gyldendal.Jini.SalesConfigurationServices.BusinessLayer;
using Gyldendal.Jini.SalesConfigurationServices.Models;
using Gyldendal.Jini.SalesConfigurationServices.Properties;
using LoggingManager;

namespace Gyldendal.Jini.SalesConfigurationServices.Controllers.Api
{
    /// <summary>
    /// Expose Jini Services methods
    /// </summary>
    [EnableCors("*", "*", "*")]
    public class SalesConfigurationController : ApiController
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public SalesConfigurationController()
        {
            _businessLayerFacade = new BusinessLayerFacade();
        }
        #endregion
        
        #region SalesConfiguration


        /// <summary>Verify if Sales Configuration exists for a particular ISBN</summary>
        /// <param name="isbn" type="string"></param>
        /// <returns>boolean value</returns>
        [Route("api/v1/SalesConfiguration/Published/{isbn}")]
        [HttpGet]
        public IHttpActionResult SalesConfigurationPublished(string isbn)
        {
            try
            {
                return Json(_businessLayerFacade.IsPublished(isbn, Settings.Default.JiniServiceUrl, Settings.Default.Seller));
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                var ex = e.GetType();
                if (ex == typeof(NullReferenceException))
                    return BadRequest("You made an Invalid Request!");
                return InternalServerError();
            }

        }

        /// <summary>
        /// Verify if Sales Configuration exists for a list of ISBNs
        /// </summary>
        /// <param name="isbn" type="Array of String"></param>
        /// <returns>List of boolean values</returns>
        [Route("api/v1/SalesConfiguration/Published")]
        [HttpPost]
        public IHttpActionResult SalesConfigurationPublished(List<string> isbn)
        {
            try
            {
                Dictionary<string, bool> results = new Dictionary<string, bool>();
                foreach (var value in isbn)
                {
                    results.Add(value,_businessLayerFacade.IsPublished(value, Settings.Default.JiniServiceUrl, Settings.Default.Seller));
                }
                return Json(results);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                var ex = e.GetType();
                if (ex == typeof(NullReferenceException))
                    return BadRequest("You made an Invalid Request!");
                return InternalServerError();
            }

        }


        /// <summary>
        /// Get Sales Configuration for an ISBN
        /// </summary>
        /// <param name="isbn" type="string"></param>
        /// <param name="institutionNumber" type="string"></param>
        /// <returns>SalesConfiguration object</returns>
        /// 
        [Route("api/v1/SalesConfiguration/Get/{isbn}/InstitutionNumber/{institutionNumber}")]
        [HttpGet]
        public IHttpActionResult GetSalesConfiguration(string isbn, string institutionNumber)
        {
            try
            {
                return Json(_businessLayerFacade.GetConfiguration(isbn, institutionNumber, Settings.Default.JiniServiceUrl, Settings.Default.UniCServiceUrl, Settings.Default.Seller));
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                var ex = e.GetType();
                if (ex == typeof(NullReferenceException))
                    return BadRequest("You made an Invalid Request!");
                return InternalServerError();
            }

        }


        /// <summary>
        /// Return Sales Configurations for a List of ISBN specific to a single institution.
        /// </summary>
        /// <param name="isbn" type="Array of String"></param>
        /// <param name="institutionNumber" type="string"></param>
        /// <returns>List of SalesConfiguration objects</returns>
        /// 
        [Route("api/v1/SalesConfiguration/Get/InstitutionNumber/{institutionNumber}")]
        [HttpPost]
        public IHttpActionResult GetSalesConfiguration(List<string> isbn, string institutionNumber)
        {
            try
            {
                Dictionary<string, SalesConfiguration> results = new Dictionary<string, SalesConfiguration>();
                foreach (var value in isbn)
                {
                    results.Add(value,_businessLayerFacade.GetConfiguration(value, institutionNumber, Settings.Default.JiniServiceUrl, Settings.Default.UniCServiceUrl, Settings.Default.Seller));
                }
                return Json(results);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                var ex = e.GetType();
                if (ex == typeof(NullReferenceException))
                    return BadRequest("You made an Invalid Request!");
                return InternalServerError();
            }

        }
        #endregion

        #region Private Variables
        private readonly BusinessLayerFacade _businessLayerFacade;
        private Logger Logger => Logger.Instance;
        #endregion

    }
}