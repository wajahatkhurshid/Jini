using System;

namespace Gyldendal.Jini.Services.Common
{
    public static class Enums
    {
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