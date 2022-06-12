namespace Gyldendal.Jini.Services.Common.Dtos
{
    public class PeriodPriceDto
    {
        public int Id { get; set; }
        public int UnitValue { get; set; }
        public int RefPeriodTypeCode { get; set; }
        public int AccessFormId { get; set; }
        public string Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceVat { get; set; }
        public int VatValue { get; set; }
        public bool IsCustomPeriod { get; set; }
    }
}
