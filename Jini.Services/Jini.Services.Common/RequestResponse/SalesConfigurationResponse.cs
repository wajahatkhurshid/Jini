using System.Collections.Generic;
using Gyldendal.Jini.Services.Common.Dtos;

namespace Gyldendal.Jini.Services.Common.RequestResponse
{
    public class SalesConfigurationResponse
    {
        public string Isbn { get; set; }
        public bool IsUpdated { get; set; }
        public string RowId { get; set; }

    }
}
