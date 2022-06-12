using System;

namespace Gyldendal.Jini.ExternalClients.Gpm
{
    public class GpmConfiguration
    {
        public Uri BaseUri => new Uri(System.Configuration.ConfigurationManager.AppSettings["GpmUrl"]);
        public string Username => "swagger";
        public string Role => "SuperAdmin";
    }
}
