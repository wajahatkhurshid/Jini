using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gyldendal.Jini.Web.Models.Common;

namespace Gyldendal.Jini.Web.Models.License
{
    class License
    {
        [Required]
        public AccessType access { get; set; }

        [Required]
        public List<SalesType> sales { get; set; }

        [Required]
        public List<Period> period  { get; set; }

        [Required]
        public List<BillingModel> billingConfiguration { get; set; }
        
    }
}
