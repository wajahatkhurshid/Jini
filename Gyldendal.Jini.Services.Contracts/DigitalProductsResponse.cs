using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Contracts
{
   public class DigitalProductsResponse
    {
        public int Total { get; set; }
        public List<DigitalProduct> Data { get; set; }
    }
}
