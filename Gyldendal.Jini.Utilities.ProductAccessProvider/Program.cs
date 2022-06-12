using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using CsvHelper;
using Gyldendal.Jini.Utilities.ProductAccessProvider.Model;
using Newtonsoft.Json;
using Configuration = CsvHelper.Configuration.Configuration;

namespace Gyldendal.Jini.Utilities.ProductAccessProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Adding Access Provider");
            AddProductAccessProvider();
            Console.WriteLine("Done");
        }
        public static void AddProductAccessProvider()
        {
            string startupPath = Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(startupPath).Parent.FullName;
            string file = projectDirectory + "/Sample/Book1.csv";
            List<Product> product;
            HttpClient client = new HttpClient();
            try
            {
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, new Configuration() { Delimiter = "," }))

                {
                    product = csv.GetRecords<Product>().ToList();
                }

                foreach (var q in product)
                {
                    if (!string.IsNullOrWhiteSpace(q.EKey))
                    {
                        var accessProvider = new List<int>();
                        string url = ConfigurationManager.AppSettings["JiniServiceUrl"] +
                                     "v1/Product/SaveProductAccessProviders/";
                        if (!string.IsNullOrWhiteSpace(q.EKey))
                            accessProvider.Add((int) Utility.AccessProvider.Ekey);
                        var productRequest = new ProductRequest();

                        productRequest.Isbn = q.ISBN;
                        productRequest.IsExternalLogin = false;
                        productRequest.AccessProvider = accessProvider.ToArray();


                        var json = JsonConvert.SerializeObject(productRequest);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = client.PostAsync(url, data);

                        string result = response.Result.ToString();
                        Console.WriteLine($"Product Isbn = {q.ISBN} is Added with AccessProvider {(int)Utility.AccessProvider.Ekey}");
                        Console.WriteLine(result);

                        //if (!string.IsNullOrWhiteSpace(q.EKey))
                        //{
                        //    var value = (int)Utility.AccessProvider.Ekey;
                        //    string uri = ConfigurationManager.AppSettings["JiniServiceUrl"] +
                        //                 "v1/Product/SaveProductAccessProvider/?isbn=" + q.ISBN + "&enumAccessProvider=" + value;
                        //    //uri=   $"http://qa-jiniservices.gyldendal.local/api/v1/Product/SaveProductAccessProvider/?isbn={q.ISBN}&enumAccessProvider={value}";
                        //    client.GetAsync(uri).GetAwaiter().GetResult();
                        //    Console.WriteLine($"Product with Isbn {q.ISBN} with access Provider {value} added");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Some thing went wrong");
            }
        }
    }

}
