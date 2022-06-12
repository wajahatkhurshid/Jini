using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.IO;

namespace SaleConfiguration_Operations
{
    public static class CsvDataReader
    {
        public static DataSet Read(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found please make sure file exists on given path and you spell path correctly.");

            var csvTable = new DataTable();
            using (CsvReader csv = new CsvReader(new StreamReader(filePath), true))
            {
                csvTable.Load(csv);
            }

            var csvDataSet = new DataSet();
            csvDataSet.Tables.Add(csvTable);
            return csvDataSet;
        }
    }
}