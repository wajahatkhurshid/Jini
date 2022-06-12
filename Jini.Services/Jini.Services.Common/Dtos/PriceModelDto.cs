namespace Gyldendal.Jini.Services.Common.Dtos
{
    public class PriceModelDto
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceVat { get; set; }
    }
}
