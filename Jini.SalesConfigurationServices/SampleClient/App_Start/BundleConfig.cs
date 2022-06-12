using System.Web.Optimization;

namespace Gyldendal.Jini.SalesConfigurationServices.SampleClient
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-1.10.2.min.js"));


            bundles.Add(new Bundle("~/bundles/Jini").Include(
                      "~/Scripts/angular.min.js",
                      "~/Scripts/jini/jini.js",
                      "~/Scripts/jini/Core/Util.js",
                      "~/Scripts/jini/services/salesconfiguration.js",
                      "~/Scripts/jini/controllers/wizard.js"
                     
                      ));

            bundles.Add(new Bundle("~/Content/Jini").Include(
                      ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
