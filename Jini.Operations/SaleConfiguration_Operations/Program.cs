using System;
using System.IO;

namespace SaleConfiguration_Operations
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Please enter complete file path with file name and extension like: D:\\Jini-SalesConfiguration-prices.csv");
            var filePath = Console.ReadLine();
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File doesn't exists on specified path!! Please restart utility and enter correct path.");
                Console.WriteLine("Press any key to end...");
                Console.ReadKey();
                return;
            }

            //Console.WriteLine("Please enter sheet name like : Ark1");
            //var sheetName = Console.ReadLine();

            Console.WriteLine(DateTime.Now + " Starting processing for file: " + filePath + "\n it will take some time...");

            var saleConfigurationProcessor = new SaleConfigurationProcessor();
            saleConfigurationProcessor.Process(filePath);

            Console.WriteLine(DateTime.Now + " Completed processing for file: " + filePath);
            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
        }
    }
}