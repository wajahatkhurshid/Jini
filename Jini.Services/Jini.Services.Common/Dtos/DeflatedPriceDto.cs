using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Common.Dtos
{
    public class DeflatedPriceDto
    {
        public string Isbn { get; set; }
        public string Id { get; set; }
        public List<PeriodPriceDto> PeriodPrices { get; set; }
    }
}
