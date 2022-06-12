using Gyldendal.Jini.SalesConfigurationServices.SampleClient;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Gyldendal.Jini.SalesConfigurationServices.SampleClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
