namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    public class Period : RefEntity
    {
        public int UnitValue { get; set; }
        public int RefPeriodUnitTypeCode { get; set; }
        public int RefSalesFormCode { get; set; }
        public int Id { get; set; }
        public Price Price { get; set; }
        public string Currency { get; set; } = "DK"; //todo: get information from UI
        public bool IsCustomPeriod { get; set; }
    }

    public class Price
    {
        //Price without Tax
        public decimal UnitPrice { get; set; }
        //Price with Tax
        public decimal UnitPriceVat { get; set; }

        public decimal CalculatedPrice { get; set; }
        // Tax 
        public int VatValue { get; set; }
    }

    public class PeriodUnitType : RefEntity
    {
    }
}