using System.Collections.Generic;

namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    public class SalesConfiguration
    {
        public List<SalesForm> SalesForms { get; set; }
        public string Isbn { get; set; }
        public List<AccessForm> AccessForms { get; set; }


    }
}