namespace Gyldendal.Jini.Web.Common
{
    public static class Enums
    {
        public enum EnumAccessForm
        {
            School=1001,
            Class=1002,
            Teacher=1003,
            SingleUser=1004,
            ContactSales=1005
        }

        public enum EnumSalesForm
        {
            Subscription = 1001,
            Rental = 1002
        }

        public enum EnumPeriodUnitType
        {
            Year = 1001,
            Month =  1002
        }

        public enum EnumPriceModel
        {
            WholeSchool = 1001,
            NoOfStudents = 1002,
            PercentageOfStudents = 1003,
            NoOfStudentsInReleventClasses = 1004,
            NoOfReleventClasses = 1005,
            NoOfStudentsInClasses = 1006,
            NoOfClasses = 1007
        }

        public enum EnumPeriodType
        {
            Binding = 1001,
            Access = 1002
        }
        public enum EnumState
        {
            Draft = 1001,
            Approved = 1002
        }
    }
}