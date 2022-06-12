using Gyldendal.Jini.SalesConfigurationServices.Models;

namespace Gyldendal.Jini.SalesConfigurationServices.BusinessLayer.Interfaces
{
    public interface ISalesConfiguration
    {
        bool IsPublished(string isbn, string jiniServiceUrl, string seller);

        SalesConfiguration GetConfiguration(string isbn, string institutionNumber, string jiniServiceUrl, string uniCServiceUrl, string seller);
        
    }
}
