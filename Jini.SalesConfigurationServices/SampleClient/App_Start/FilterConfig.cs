using System.Web.Mvc;

namespace Gyldendal.Jini.SalesConfigurationServices.SampleClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
