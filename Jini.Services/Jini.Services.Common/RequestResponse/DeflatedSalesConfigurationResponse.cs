using System.Collections.Generic;
using Gyldendal.Jini.Services.Common.Dtos;

namespace Gyldendal.Jini.Services.Common.RequestResponse
{
    public class DeflatedSalesConfigurationResponse
    {
        public int Total { get; set; }
        public int ProductCount { get; set; }
        public List<DeflatedSalesConfigurationDto> Data { get; set; }

    }
}
