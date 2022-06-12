using Autofac;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gyldendal.Jini.Services
{
    /// <summary>
    ///
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register); // Implemented at the bottom, NR
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            HttpConfiguration config = GlobalConfiguration.Configuration;

            IContainer container = new ContainerRegistration().Initialize(config);

            GlobalConfiguration.Configure(cf => WebApiConfig.Register(config));
        }
    }
}