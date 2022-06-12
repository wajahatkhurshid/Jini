using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.ShopServices.Contracts.License.Access;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration.Response;
using Gyldendal.Common.WebUtils.HttpClient;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.ApiClient
{
    public class JiniManager : IJiniManager
    {
        private HttpClientUtility _httpClientUtility;
        private static string BaseUrl { get; set; }

        private static Dictionary<string, string> Header { get; set; }

        private HttpClientUtility HttpClient => _httpClientUtility ?? (_httpClientUtility = new HttpClientUtility(BaseUrl, Header));

        public JiniManager(string jiniservice)
        {
            BaseUrl = jiniservice;
            Header = null;
        }

        public List<int> IsSalesConfigurationPublished(WebShop webShop, string isbn)
        {
            var saleConfigurationUrl = $"api/v1/SalesConfiguration/Exists/webshop/{isbn}/{webShop}";
            return HttpClient.GetAsync<List<int>>(saleConfigurationUrl);
        }

        /// <summary>
        /// Jini Api Client
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public SalesConfiguration GetProductSaleConfiguration(WebShop webShop, string isbn)
        {
            var saleConfigurationUrl = $"api/v1/SalesConfiguration/Get/WebShop/{isbn}/{webShop}/{true}";
            return HttpClient.GetAsync<SalesConfiguration>(saleConfigurationUrl);
        }

        /// <summary>
        /// Jini Api Client
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbn"></param>
        /// <param name="salesConfigType"></param>
        /// <returns></returns>
        public SalesConfiguration GetProductSaleConfiguration(WebShop webShop, string isbn, SalesConfigurationType salesConfigType)
        {
            var saleConfigurationUrl = $"api/v1/SalesConfiguration/Get/WebShop/{isbn}/{webShop}/{salesConfigType}/{true}";
            return HttpClient.GetAsync<SalesConfiguration>(saleConfigurationUrl);
        }

        /// <summary>
        /// Returns the information about Sale Configs that have been updated after the given date and for the given seller.
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <param name="webShop"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<UpdatedSaleConfigInfo> GetUpdatedSaleConfigsInfo(long updatedAfter, WebShop webShop, int take)
        {
            var queryString = $"api/v1/SalesConfiguration/GetUpdatedSaleConfigsInfo/{updatedAfter}/{webShop}/{take}";

            return HttpClient.GetAsync<IEnumerable<UpdatedSaleConfigInfo>>(queryString);
        }

        /// <summary>
        ///     Get Grade Levels of ISBN from RAP
        /// </summary>
        /// <returns>returns Json result of GradeLevels</returns>
        public string GetGradeLevels(string isbn)
        {
            var queryString = $"api/v1/Product/GetGradeLevels/{isbn}";

            return HttpClient.GetAsync<string>(queryString);
        }

        /// <summary>
        ///  Tells either the specified accessProvider exists for isbn or not
        /// </summary>
        /// <returns></returns>
        public bool IsProductAccessProviderExists(string isbn, EnumAccessProvider enumAccessProvider)
        {
            var queryString = $"api/v1/Product/IsProductAccessProviderExists/{isbn}/{enumAccessProvider}";

            return HttpClient.GetAsync<bool>(queryString);
        }

        /// <summary>
        ///  Return the list of product accessProviders of isbns
        /// </summary>
        /// <returns></returns>
        public List<ProductAccessProvider> GetProductAccessProviders(List<string> isbns)
        {
            var queryString = $"api/v1/Product/GetProductAccessProviders/";

            return HttpClient.PostAsync<List<ProductAccessProvider>, List<string>>(queryString, isbns);
        }
    }
}