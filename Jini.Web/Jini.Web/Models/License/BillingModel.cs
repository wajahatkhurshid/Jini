using Gyldendal.Jini.Web.Models.Common;

namespace Gyldendal.Jini.Web.Models.License
{
    public class BillingModel
    {
        
        public SalesType salestype { get; set; }
        public string label { get; set; }

        public double percentage { get; set; }
        public double unitPrice { get; set; }
        public double unitPriceInclVAT { get; set; }
    }
}
