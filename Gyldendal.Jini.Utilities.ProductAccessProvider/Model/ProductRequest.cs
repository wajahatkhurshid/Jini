using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Utilities.ProductAccessProvider.Model
{
   public class ProductRequest
    {
        public string Isbn { get; set; }
        public bool IsExternalLogin { get; set; }
        public int[] AccessProvider { get; set; }
    }
}
