using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Interfaces;
using Gyldendal.Jini.SalesConfigurationServices.Common;
using Gyldendal.Jini.SalesConfigurationServices.Models;

namespace Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Controllers
{
    public class SalesConfigurationController : ISalesConfiguration
    {
        public SalesConfiguration GetConfiguration(string isbn, string institutionNumber, string jiniServiceUrl, string uniCServiceUrl, string seller)
        {
            if (isbn == null || institutionNumber == null)
                throw new NullReferenceException();
            var configuration = Util.GetAsync<SalesConfiguration>(jiniServiceUrl, "v1/SalesConfiguration/Get/" + isbn + "/" + seller + "/true");
            if (configuration == null)
            {
                throw new NullReferenceException();
            }
            var klasseConfiguration = configuration.AccessForms.FirstOrDefault(x=> x.Code == (int) Enums.EnumAccessForm.Class);
            var result = Util.GetAsync<List<Class>>(uniCServiceUrl, "v1/Unic/Institution/Classes/" + institutionNumber);
            if (klasseConfiguration != null)
            {
                klasseConfiguration.PriceModels[0].Classes = new List<Class>();
                klasseConfiguration.PriceModels[0].Classes.AddRange(result);
            }
            
            return configuration;
        }

        public bool IsPublished(string isbn, string jiniServiceUrl, string seller)
        {
            if (isbn == null)
                throw new NullReferenceException();
            return Util.GetAsync<List<int>>(jiniServiceUrl, "v1/SalesConfiguration/Exists/" + isbn + "/"+ seller +"/true").Any(x=>x == (int)Enums.EnumState.Approved);   
        }
        
    }
}
