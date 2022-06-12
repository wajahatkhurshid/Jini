using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.Jini.Services.Common.Dtos;

namespace Gyldendal.Jini.Services.Common.RequestResponse
{
  public  class DeflatedSalesConfigurationV2Response
    {
        public int Total { get; set; }
        public int ProductCount { get; set; }
        public List<DeflatedSalesConfigurationV2Dto> Data { get; set; }
    }
}
