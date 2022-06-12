namespace Gyldendal.Jini.Services.Common.ConfigurationManager
{
    public interface IJiniConfigurationManager
    {
        #region WebShops BaseUrls
        string GuBaseUrl { get; }
        string MunksGaardBaseUrl { get; }
        string HansReitzelUrl { get; }
        #endregion WebShops BaseUrls
        string ActiveConfiguration { get; }

        string InactiveConfiguration { get; }

        string JiniDbConnectionString { get; }

        string CacheDuration { get; }

        string RapMetaServiceUrl { get; }

        string ConnectionString { get; }

        string DraftConfiguration { get; }
        string DepartmentsToExclude { get; }
        string SectionsToExclude { get; }

    }
}