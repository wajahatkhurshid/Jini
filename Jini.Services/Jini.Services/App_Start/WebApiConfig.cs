using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using Gyldendal.Jini.Services.Filters;

namespace Gyldendal.Jini.Services
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.EnableCors();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //// Add model null value filter
            //var nullValueFilter = (NullValueFilter)config.DependencyResolver.GetService(typeof(NullValueFilter));
            //config.Filters.Add(nullValueFilter);

            var exceptionFilter = (ExceptionFilter)config.DependencyResolver.GetService(typeof(ExceptionFilter));
            config.Filters.Add(exceptionFilter);
        }
    }
}