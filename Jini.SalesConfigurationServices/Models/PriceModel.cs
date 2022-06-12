using System.Collections.Generic;

namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    public class PriceModel : RefEntity
    {
        public int RefAccessFormCode { get; set; }
        public bool ShowPercentage { get; set; }

        public int? PercentageValue { get; set; }

        public string GradeLevels { get; set; }

        public List<Class> Classes { get; set; }
    }
}