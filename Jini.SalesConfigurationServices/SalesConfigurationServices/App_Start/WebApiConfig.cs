using System.Web.Http;
using Gyldendal.Jini.SalesConfigurationServices.CustomAttributes;

namespace Gyldendal.Jini.SalesConfigurationServices
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new JiniHandleError());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnableCors();
        }
    }
}
