using Gyldendal.Jini.Services.Common.Dtos;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Common.RequestResponse
{
    public class PriceModelRequest
    {
        public string Isbn { get; set; }
        public int RefSalesconfigTypeCode { get; set; }
        public List<PriceModelDto> DeflatedPrice { get; set; }
    }
}
