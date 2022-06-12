using System;
using System.Web.Http;
using System.Web.Http.Cors;
using Gyldendal.Jini.SalesConfigurationServices.BusinessLayer;
using Gyldendal.Jini.SalesConfigurationServices.Models.Request;
using Gyldendal.Jini.SalesConfigurationServices.Properties;
using LoggingManager;
using Newtonsoft.Json;

namespace Gyldendal.Jini.SalesConfigurationServices.Controllers.Api
{
    /// <summary>
    /// Price calculation methods
    /// </summary>
    [EnableCors("*","*","*")]
    public class PriceController : ApiController
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public PriceController()
        {
            _businessLayerFacade = new BusinessLayerFacade();
        }
        #endregion

        #region Price Calculation
        /// <summary>
        /// Calculate Prices for a Given Configuration against an ISBN and institution Number
        /// </summary>
        /// <param name="isbn" type="string"></param>
        /// <param name="institutionNumber" type="string"></param>
        /// <param name="config" type="PriceRequest">Optional</param>
        /// <returns>Returns calculated prices</returns>
        [Route("api/v1/SalesConfiguration/Price/{isbn}/{institutionNumber}")]
        [HttpPost]
        public IHttpActionResult GetPrice(string isbn, string institutionNumber, PriceRequest config)
        {
            try
            {
                if (config == null)
                {
                    var serialized = JsonConvert.SerializeObject(_businessLayerFacade.GetConfiguration(isbn, institutionNumber,Settings.Default.JiniServiceUrl, Settings.Default.UniCServiceUrl, Settings.Default.Seller));
                    config = JsonConvert.DeserializeObject<PriceRequest>(serialized);
                }

                return Json(_businessLayerFacade.GetPrice(isbn, institutionNumber, Settings.Default.UniCServiceUrl, config));
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