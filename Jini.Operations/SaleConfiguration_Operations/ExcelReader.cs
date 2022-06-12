//using System.Data;
//using System.Data.OleDb;

//namespace SaleConfiguration_Operations
//{
//    internal class ExcelReader
//    {
//        private DataTable GetDataTable(string sql, string connectionString)
//        {
//            DataTable datatable = null;

//            using (OleDbConnection conn = new OleDbConnection(connectionString))
//            {
//                conn.Open();
//                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
//                {
//                    using (OleDbDataReader rdr = cmd.ExecuteReader())
//                    {
//                        datatable.Load(rdr);
//                        return datatable;
//                    }
//                }
//            }
//        }

//        public DataRowCollection GetDataRowsFromExcel(string filePath, string sheetName)
//        {
//            string fullPathToExcel = filePath; //ie C:\Temp\YourExcel.xls
//            string connString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=yes;'", fullPathToExcel);
//            DataTable dt = GetDataTable("SELECT * from [" + sheetName + "]", connString);

//            return dt.Rows;
//            //foreach (DataRow dr in dt.Rows)
//            //{
//            //    //Do what you need to do with your data here
//            //}
//        }
//    }
//}