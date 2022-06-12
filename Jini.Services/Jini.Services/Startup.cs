using System.Threading.Tasks;
using Gyldendal.Jini.Services.Hangfire;
using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(Gyldendal.Jini.Services.Startup))]

namespace Gyldendal.Jini.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(System.Configuration.ConfigurationManager.AppSettings["JiniConnectionString"]);
           
            HangFireStartup.PreProcessing();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            app.UseHangfireServer();
            HangFireJobs.ConfigureHangFireJobs();
           
        }
    }
}
