using System;

namespace Gyldendal.Jini.SalesConfigurationServices.Models
{
    public static class Enums
    {
        public enum EnumAccessForm
        {
            School = 1001,
            Class = 1002,
            Teacher = 1003,
            SingleUser = 1004,
            ContactSales = 1005
        }

        public enum EnumSalesForm
        {
            Subscription = 1001,
            Trial = 1002
        }

        public enum EnumPeriodUnitType
        {
            Year = 1001,
            Month = 1002,
            Days = 1003,
            Time = 1004
        }

        public enum EnumPriceModel
        {
            WholeSchool = 1001,
            NoOfStudents = 1002,
            PercentageOfStudents = 1003,
            NoOfStudentsInReleventClasses = 1004,
            NoOfReleventClasses = 1005,
            NoOfClasses = 1006,
            NoOfStudentsInClasses = 1007,
            WholeSchoolTeacher = 1008
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
        public static int ToInt(this Enum e)
        {
            return Convert.ToInt32(e);
        }
    }
}