using System;
using System.IO;

namespace SaleConfiguration_Operations
{
    public sealed class Logger
    {
        private static readonly StreamWriter Writer = new StreamWriter(".\\saleConfiguration_Operations.log", true);

        public static void InfoLog(string message, bool doFlush = false)
        {
            Writer.WriteLine(message);

            if (doFlush)
                Writer.Flush();
        }

        public static void ErrorLog(string message)
        {
            var error = "Error: " + message + Environment.NewLine;
            Writer.WriteLine(error);
        }
    }
}