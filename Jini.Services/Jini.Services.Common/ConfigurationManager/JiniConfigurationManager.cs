namespace Gyldendal.Jini.Services.Common.ConfigurationManager
{
    public class JiniConfigurationManager : IJiniConfigurationManager
    {
        #region WebShops BaseUrls
        public string GuBaseUrl => System.Configuration.ConfigurationManager.AppSettings["GuBaseUrl"];
        public string MunksGaardBaseUrl => System.Configuration.ConfigurationManager.AppSettings["MunksGaardBaseUrl"];
        public string HansReitzelUrl => System.Configuration.ConfigurationManager.AppSettings["HansReitzelBaseUrl"];
        #endregion WebShops BaseUrls

        public string ActiveConfiguration => System.Configuration.ConfigurationManager.AppSettings["ActiveConfiguration"];

        public string InactiveConfiguration => System.Configuration.ConfigurationManager.AppSettings["InactiveConfiguration"];

        public string JiniDbConnectionString => System.Configuration.ConfigurationManager.AppSettings["JiniConnectionString"];

        public string CacheDuration => System.Configuration.ConfigurationManager.AppSettings["CacheDuration"];

        public string DbServer => System.Configuration.ConfigurationManager.AppSettings["DbServer"];

        public string DbName => System.Configuration.ConfigurationManager.AppSettings["DbName"];

        public string DbIntegratedSecurity => System.Configuration.ConfigurationManager.AppSettings["DbIntegratedSecurity"];

        public string DbUser => System.Configuration.ConfigurationManager.AppSettings["DbUser"];

        public string DbPass => System.Configuration.ConfigurationManager.AppSettings["DbPass"];

        public string RapMetaServiceUrl => System.Configuration.ConfigurationManager.AppSettings["RapMetaServiceUrl"];

        public string ConnectionString => Utils.Utils.JiniConnectionString;

        public string DraftConfiguration => System.Configuration.ConfigurationManager.AppSettings["DraftConfiguration"];
        public string DepartmentsToExclude => System.Configuration.ConfigurationManager.AppSettings["DepartmentsToExclude"];
        public string SectionsToExclude => System.Configuration.ConfigurationManager.AppSettings["SectionsToExclude"];
    }
}