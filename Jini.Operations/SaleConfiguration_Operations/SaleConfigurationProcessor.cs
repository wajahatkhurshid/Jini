using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SaleConfiguration_Operations
{
    public class SaleConfigurationProcessor
    {
        private readonly SaleConfigurationDataAccess _saleConfigDataAccess = new SaleConfigurationDataAccess();
        private readonly SaleConfigurationParser _saleConfigParser = new SaleConfigurationParser();

        public void Process(string filePath)
        {
            Logger.InfoLog($" Starting process for file {filePath} at {DateTime.Now} ", true);
            var summarySheet = new Dictionary<string, bool>();

            try
            {
                // Reading excel file.
                //var dataSet = ExcelDataReader.Read(filePath);

                var dataSet = CsvDataReader.Read(filePath);
                var sheet = dataSet.Tables[0];

                // Skipping header row in loop
                var sheetRows = sheet.Rows.Cast<DataRow>().Skip(2);
                // applying filter
                //var allowedPeriodUnitValues = new List<string>{"1","2","3","6"};
                //var filteredSheetRows = sheetRows.Where(x => !string.IsNullOrEmpty(x[2].ToString()) &&
                //                                             !string.IsNullOrEmpty(x[9].ToString()) &&
                //                                             allowedPeriodUnitValues.Contains(x[9].ToString()) );
                int i = 1;
                foreach (var sheetRow in sheetRows)
                {
                    var isSuccessfullProccessed = ProcessRow(sheetRow);
                    summarySheet.Add(sheetRow[0] + "_" + sheetRow[1] + "_" + ++i, isSuccessfullProccessed);
                }

                Logger.InfoLog(Environment.NewLine + $"Process for file {filePath} finished.", true);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex.Message);
            }
            finally
            {
                WriteSummary(summarySheet);
            }
        }

        private bool ProcessRow(DataRow sheetRow)
        {
            try
            {
                var isbn = sheetRow[0].ToString();
                var id = int.Parse(sheetRow[1].ToString());

                Logger.InfoLog(Environment.NewLine + $"Reading sale configurations for isbn: {isbn} with Id: {id}", true);
                // Reading saleConfiguration from db.
                var dbSalesConfiguration = _saleConfigDataAccess.GetSalesConfiguration(id);
                if (dbSalesConfiguration == null)
                    throw new Exception($"No sale configurations found for isbn: {sheetRow[0]}");

                // Parse and update saleConfiguration
                var updatedPeriodPrice = _saleConfigParser.Parse(dbSalesConfiguration, sheetRow);
                _saleConfigDataAccess.UpdateSalesConfigurationPrice(updatedPeriodPrice);
                Logger.InfoLog($"Sale configurations updated for isbn: {isbn} with Id: {id}", true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog(ex.Message);
                return false;
            }
        }

        private void WriteSummary(Dictionary<string, bool> summarySheet)
        {
            string csv = string.Join(
                Environment.NewLine,
                summarySheet.Select(d => d.Key + "," + d.Value)
            );
            System.IO.File.WriteAllText(".\\saleConfiguration_Operations_Summary.csv", csv);

            string failedConfig = string.Join(
                Environment.NewLine,
                summarySheet.Where(x => x.Value == false).Select(d => d.Key.Split('_')[0]).Distinct()
            );
            System.IO.File.WriteAllText(".\\saleConfiguration_Operations_Falied_ISBNs.txt", failedConfig);
        }
    }
}