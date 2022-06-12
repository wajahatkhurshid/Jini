using System.Web.Optimization;

namespace Gyldendal.Jini.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/kendo/2020.3.1021/jquery.min.js"));

            bundles.Add(new Bundle("~/bundles/Jini").Include(
                      "~/Scripts/jini/jini-custom.js",
                      "~/Scripts/jini/jini-kendo-controls.js",
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-route.min.js",
                      "~/Scripts/angular-ui-router.min.js",
                      "~/Scripts/angular-breadcrumb.min.js",
                      "~/Scripts/ngclipboard.js",
                      "~/Scripts/jini/routing/routing.js",
                      "~/Scripts/jini/jini.js",
                      "~/Scripts/jini/common/config.js",
                      "~/Scripts/jini/common/util.js",
                      "~/Scripts/jini/core/validation.js",
                      "~/Scripts/jini/services/jiniservice.js",
                      "~/Scripts/jini/services/dataFactory.js",
                      "~/Scripts/jini/core/periodService.js",
                      "~/Scripts/jini/core/priceService.js",
                      "~/Scripts/jini/core/salesformService.js",
                      "~/Scripts/jini/core/accessformService.js",
                      "~/Scripts/jini/core/salesconfiguration.js",
                      "~/Scripts/jini/controllers/history.js",
                      "~/Scripts/jini/controllers/create.js",
                      "~/Scripts/jini/controllers/config.js",
                      "~/Scripts/jini/controllers/approve.js",
                      "~/Scripts/jini/controllers/published.js",
                      "~/Scripts/jini/controllers/trial.js",
                      "~/Scripts/jini/controllers/dashboard.js",
                      "~/Scripts/jini/directives/createLink.js",
                      "~/Scripts/jini/directives/AccessProvider.js",
                      "~/Scripts/jini/directives/salesetup.js",
                       "~/Scripts/jini/directives/RevisionHistory.js",
                        "~/Scripts/jini/directives/GuaRevisionHistory.js",
                      "~/Scripts/jini/Core/trialLicense.js",
                      "~/Scripts/jini/controllers/wizard.js",
                      "~/Scripts/jinibulk/jinibulk.js",
                      "~/Scripts/jinibulk/services/jinibulkService.js",
                      "~/Scripts/jinibulk/controllers/configurationCtrl.js",
                      "~/Scripts/V2/jinibulk/jiniV2bulk.js",
                      "~/Scripts/V2/jinibulk/services/jiniV2bulkService.js",
                      "~/Scripts/V2/jinibulk/controllers/configurationV2Ctrl.js",
                      "~/Scripts/kendo/2020.3.1021/kendo.all.min.js",
                      "~/Scripts/jquery.expander.min.js",
                      "~/Scripts/kendo.modernizr.custom.js",
                      "~/Scripts/kendo/2020.3.1021/cultures/kendo.culture.da-DK.min.js",
                      "~/Scripts/kendo/2020.3.1021/messages/kendo.messages.da-DK.min.js",
                      "~/Scripts/cookies.min.js",
                      "~/Scripts/blockUI.js",
                      "~/Scripts/clipboard.min.js"

                      ));
            bundles.Add(new Bundle("~/Content/Jini").Include(
                      "~/Content/style.css"));
            //BundleTable.EnableOptimizations = true;
        }
    }
}
