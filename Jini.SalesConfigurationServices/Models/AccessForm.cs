using System.Collections.Generic;

namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    public class AccessForm : RefEntity
    {
        public List<PriceModel> PriceModels { get; set; }
        public List<Period> BillingPeriods { get; set; }
        public List<Class> SelectedClasses { get; set; }
        public int? StudentCount { get; set; }
    }
}