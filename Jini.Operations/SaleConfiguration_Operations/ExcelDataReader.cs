using ExcelDataReader;
using System.Data;
using System.IO;

namespace SaleConfiguration_Operations
{
    public static class ExcelDataReader
    {
        public static DataSet Read(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found please make sure file exists on given path and you spell path correctly.");

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    return reader.AsDataSet();
                }
            }
        }
    }
}