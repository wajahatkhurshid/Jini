using Gyldendal.Jini.Utilities.TextCleanUp.DataAccess;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Gyldendal.Jini.Utilities.TextCleanUp
{
    internal class Program
    {
        private static void Main()
        {
            StreamWriter log;
            if (!File.Exists("logfile.txt")) { log = new StreamWriter("logfile.txt"); }
            else { log = File.AppendText("logfile.txt"); }

            log.WriteLine(DateTime.Now + " Starting utility...");
            Console.WriteLine("Starting utility...");
            try
            {
                using (var entities = new SalesConfiguration_Entities())
                {
                    var dbConnTest = entities.AccessForms.FirstOrDefault();

                    Console.WriteLine("Utility started this will take around 5-10 mins.");
                    UpdateAccessFormDescription(entities, log);
                    UpdateTrialDescription(entities, log);
                }
                Console.WriteLine("Utility finished successfully.");
                log.WriteLine("Finished successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong, Exception: " + e.Message);
                log.WriteLine("Something went wrong, Exception: " + e.Message + "\n" + JsonConvert.SerializeObject(e));
            }
            finally
            {
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
                
                log.WriteLine("\nClosing\n\n");
                log.Close();
            }
        }

        private static void UpdateTrialDescription(SalesConfiguration_Entities entities, StreamWriter log)
        {
            var doc = new HtmlDocument();

            var trialContactSales = entities.TrialLicenses.Where(x => !string.IsNullOrEmpty(x.ContactSalesText)).ToList();
            Console.WriteLine("\nStarted updating trialLicenses description, total records: " + trialContactSales.Count);
            log.WriteLine("\nStarted updating trialLicenses description, total records: " + trialContactSales.Count + "\n");

            foreach (var trialContactSale in trialContactSales)
            {
                log.WriteLine("Updating trialId : " + trialContactSale.Id + ", text : " + trialContactSale.ContactSalesText);

                doc.LoadHtml(trialContactSale.ContactSalesText);
                HtmlPackUtil.RemoveComments(doc.DocumentNode);
                HtmlPackUtil.RemoveStyleAttributes(doc);

                var modifiedText = doc.DocumentNode.OuterHtml;
                log.WriteLine("Updated trialId : " + trialContactSale.Id + ", modified text : " + modifiedText);

                if (!string.Equals(modifiedText, trialContactSale.ContactSalesText))
                {
                    trialContactSale.ContactSalesText = modifiedText;
                    entities.SaveChanges();
                }
            }
        }

        private static void UpdateAccessFormDescription(SalesConfiguration_Entities entities, StreamWriter log)
        {
            var doc = new HtmlDocument();

            var relativeAccessForms = entities.AccessForms.Where(x => x.RefCode == 1005).ToList();
            //var relativeAccessForms = entities.AccessForms.Where(x => x.Id == 30).ToList();

            Console.WriteLine("\nStarted updating licenses description, total records: " + relativeAccessForms.Count);
            log.WriteLine("\nStarted updating licenses description, total records: " + relativeAccessForms.Count + "\n");

            foreach (var relativeAccessForm in relativeAccessForms)
            {
                log.WriteLine("Updating accessFormId : " + relativeAccessForm.Id + ", text : " + relativeAccessForm.DescriptionText);

                doc.LoadHtml(relativeAccessForm.DescriptionText);

                HtmlPackUtil.RemoveComments(doc.DocumentNode);
                HtmlPackUtil.RemoveStyleAttributes(doc);

                var modifiedText = doc.DocumentNode.OuterHtml;
                log.WriteLine("Updated accessFormId : " + relativeAccessForm.Id + ", modified text : " + modifiedText);

                if (!string.Equals(modifiedText, relativeAccessForm.DescriptionText))
                {
                    relativeAccessForm.DescriptionText = modifiedText;
                    entities.SaveChanges();
                }
            }
        }
    }
}