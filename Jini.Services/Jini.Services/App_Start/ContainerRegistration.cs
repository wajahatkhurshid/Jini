using Autofac;
using Autofac.Integration.WebApi;
using Gyldendal.Jini.Services.Common.ConfigurationManager;
using Gyldendal.Jini.Services.Common.Services;
using Gyldendal.Jini.Services.Common.Utils;
using Gyldendal.Jini.Services.Core.Manager;
using Gyldendal.Jini.Services.Core.Product;
using Gyldendal.Jini.Services.Core.SaleConfiguration;
using Gyldendal.Jini.Services.Core.SaleConfiguration.Reader;
using Gyldendal.Jini.Services.Core.Seller;
using Gyldendal.Jini.Services.Core.TrialLicense;
using Gyldendal.Jini.Services.Data;
using Gyldendal.Jini.Services.Data.DataAccess;
using Gyldendal.Jini.Services.Filters;
using Gyldendal.Jini.Services.Utils;
using Gyldendal.Jini.Utilities.Caching;
using Gyldendal.Jini.Utilities.Caching.Interfaces;
using System.Reflection;
using System.Web.Http;
using Gyldendal.Jini.ExternalClients.AX;
using Gyldendal.Jini.ExternalClients.Gpm;
using Gyldendal.Jini.ExternalClients.Interfaces;
using Gyldendal.Jini.Repository;
using Gyldendal.Jini.Repository.Contracts;
using Gyldendal.Jini.Services.Core.Departments;
using Gyldendal.Jini.Services.Core.GpmSubscription;
using Gyldendal.Jini.Services.Core.MediaMaterialType;
using Gyldendal.Jini.Services.Core.Product.Services;
using Hangfire;
using GlobalConfiguration = Hangfire.GlobalConfiguration;
using LoggingManager.Entities;


namespace Gyldendal.Jini.Services
{
    /// <summary>
    /// Registers the Autofac as container for Dependency Injection
    /// </summary>
    public class ContainerRegistration
    {

        /// <summary>
        /// Initializes the Autofac container
        /// </summary>
        /// <param name="config"></param>
        public IContainer Initialize(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            //settings.Initialize(Settings.Default.ActiveConfiguration, Settings.Default.InactiveConfiguration);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            //todo: if we are registering all types manually then do we need to register Assembly types?
            RegisterDatabaseEntities(builder);
            RegisterManager(builder);
            RegisterUtilities(builder);
            RegisterDbController(builder);
            RegisterFacade(builder);
            RegisterClient(builder);
            RegisterRepositories(builder);
            RegisterServices(builder); // Repeat for each assembly
            IContainer container = builder.Build();
            GlobalConfiguration.Configuration.UseAutofacActivator(container);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());
            builder.RegisterType<RapService>().As<IRapService>();
            builder.RegisterType<GpmSubscriptionService>().As<IGpmSubscriptionService>();
            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<GpmMessageConsumerService>().As<IGpmMessageConsumerService>();
        }
        private static void RegisterClient(ContainerBuilder builder)
        {
            builder.RegisterType<GpmApiClient>().As<IGpmApiClient>();
            builder.RegisterType<GpmConfiguration>();

            builder.RegisterType<AxApiClient>().As<IAxApiClient>();
            builder.RegisterType<AxConfigurations>();
        }
        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<MediaMaterialTypeService>().As<IMediaMaterialTypeService>();
            builder.RegisterType<DepartmentsService>().As<IDepartmentsService>();
            builder.RegisterType<TrialLicenseRepository>().As<ITrialLicenseRepository>();
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();
            builder.RegisterType<ProductRepository>().As<IProductRepository>();
            builder.RegisterType<MediaMaterialTypeRepository>().As<IMediaMaterialTypeRepository>();
            builder.RegisterType<GpmSubscriptionRepository>().As<IGpmSubscriptionRepository>();
           
        }
        private static void RegisterFacade(ContainerBuilder builder)
        {
            builder.RegisterType<ProductFacade>().As<IProductFacade>();
            builder.RegisterType<ProductAccessProviderFacade>().As<IProductAccessProviderFacade>();
            builder.RegisterType<TrialLicenseFacade>().As<ITrialLicenseFacade>();
            builder.RegisterType<SellerFacade>().As<ISellerFacade>();
            builder.RegisterType<SaleConfigurationFacade>().As<ISaleConfigurationFacade>();
            builder.RegisterType<DeflatedSalesConfigurationFacade>().As<IDeflatedSalesConfigurationFacade>();
        }
        private static void RegisterDbController(ContainerBuilder builder)
        {
            builder.RegisterType<LookUpsDbController>().As<ILookUpsDbController>();
            builder.RegisterType<SaleConfigurationDbController>().As<ISaleConfigurationDbController>();
            builder.RegisterType<ProductAccessProviderDbController>().As<IProductAccessProviderDbController>();
            builder.RegisterType<DeflatedSalesConfigurationDbController>().As<IDeflatedSalesConfigurationDbController>();
        }
        private static void RegisterUtilities(ContainerBuilder builder)
        {
            builder.RegisterType<LogWrapper>().As<ILogger>().SingleInstance();
            builder.RegisterType<LoggingManager.Logger>().As<LoggingManager.ILogger>().SingleInstance();
            builder.RegisterType<LoggingManagerConfig>().As<ILoggingManagerConfig>().SingleInstance();
            builder.RegisterType<ServiceHelper>().As<IServiceHelper>();
            builder.RegisterType<ErrorCodeUtil>().As<IErrorCodeUtil>();
            builder.RegisterType<ExceptionFilter>().As<IExceptionFilter>();
            builder.RegisterType<DefaultCache>().As<ICacher>();
        }
        private static void RegisterManager(ContainerBuilder builder)
        {
            builder.RegisterType<RapManager>().As<IRapManager>();
            builder.RegisterType<JiniManager>().As<IJiniManager>();
            builder.RegisterType<JiniConfigurationManager>().As<IJiniConfigurationManager>();
            builder.RegisterType<SaleConfigurationReader>().As<ISaleConfigurationReader>();
        }
        private static void RegisterDatabaseEntities(ContainerBuilder builder)
        {
            builder.Register(ctx =>
                new Jini_Entities(Common.Utils.Utils.JiniConnectionString)).InstancePerLifetimeScope();
        }
    }
}