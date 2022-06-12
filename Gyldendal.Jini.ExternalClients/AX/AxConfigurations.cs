using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Jini.ExternalClients.AX
{
    public class AxConfigurations
    {
        public Uri BaseUri => new Uri(System.Configuration.ConfigurationManager.AppSettings["AxApiUrl"]);
    }
}
