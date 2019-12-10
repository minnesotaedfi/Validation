using System;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Engine.Language;
using Engine.Models;
using Engine.Utility;
using log4net;
using log4net.Config;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using ValidationWeb.ApiControllers.ModelBinders;
using ValidationWeb.Database;
using ValidationWeb.DataCache;
using ValidationWeb.Filters;
using ValidationWeb.Models;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb
{
    public class Global : HttpApplication
    {
        private const string LoggerName = "ValidationPortalLogger";

        public static string ApiUrlPrefixRelative => "~/api";

        protected void Application_Start(object sender, EventArgs e)
        {
            ConfigureLogging();

            // Configure MVC
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = ConfigureDependencyInjection();
            GlobalFilters.Filters.Add(new PortalAuthenticationFilter(container));

            // Configure the Web API
            GlobalConfiguration.Configure(ConfigureWebApi);

            // set up model binding for dataTable requests
            DataTables.AspNet.Mvc5.Configuration.RegisterDataTables();
        }

        /// <summary>
        /// Writes log to C:\ProgramData\ValidationPortal\ValidationPortal.log - or wherever Program Data environmental variable points to.
        /// </summary>
        protected virtual void ConfigureLogging()
        {
            XmlConfigurator.Configure();
        }

        protected Container ConfigureDependencyInjection()
        {
            // Create a Simple Injector container, and register concrete instances.
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            // This is an extension method from the MVC integration package.
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            AddAppServicesToContainer(container);

            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            return container;
        }

        protected void AddAppServicesToContainer(Container container)
        {
            // Web Portal
            // scoped
            container.Register<IAnnouncementService, AnnouncementService>(Lifestyle.Scoped);
            container.Register<IAppUserService, AppUserService>(Lifestyle.Scoped);
            container.RegisterInstance<IConfigurationValues>(new AppSettingsFileConfigurationValues());
            container.Register<IEdOrgService, EdOrgService>(Lifestyle.Scoped);
            container.Register<IHttpContextProvider, HttpContextProvider>(Lifestyle.Scoped);
            container.Register<ISchoolYearService, SchoolYearService>(Lifestyle.Scoped);
            container.Register<IValidationResultsService, ValidationResultsService>(Lifestyle.Scoped);
            container.Register<ISubmissionCycleService, SubmissionCycleService>(Lifestyle.Scoped);
            container.Register<IEdFiApiLogService, EdFiApiLogService>(Lifestyle.Scoped);
            container.Register<IRecordsRequestService, RecordsRequestService>(Lifestyle.Scoped);
            container.Register<IOdsConfigurationValues, OdsConfigurationValues>(Lifestyle.Singleton);
            container.Register<IDynamicReportingService, DynamicReportingService>(Lifestyle.Scoped);
            container.Register<IManualRuleExecutionService, ManualRuleExecutionService>(Lifestyle.Scoped);

            // singletons
            container.Register<ICacheManager, CacheManager>(Lifestyle.Singleton);
            container.Register<IDbContextFactory<ValidationPortalDbContext>, DbContextFactory<ValidationPortalDbContext>>(Lifestyle.Singleton);
            container.Register<IDbContextFactory<EdFiLogDbContext>, DbContextFactory<EdFiLogDbContext>>(Lifestyle.Singleton);
            container.Register<ISchoolYearDbContextFactory, SchoolYearDbContextFactory>(Lifestyle.Singleton);

            // Rules Engine
            container.Register<ISchemaProvider, EngineSchemaProvider>(Lifestyle.Scoped);
            container.Register<IRulesEngineService, RulesEngineService>(Lifestyle.Scoped);
            container.Register<IRulesEngineConfigurationValues, RulesEngineConfigurationValues>(Lifestyle.Scoped);

            var rulesEngineConfigurationValues = new RulesEngineConfigurationValues();
            var asConfiguredRulesDirectory = rulesEngineConfigurationValues.RulesFileFolder;
            if (asConfiguredRulesDirectory.StartsWith("~"))
            {
                asConfiguredRulesDirectory = Server.MapPath(asConfiguredRulesDirectory);
            }

            var configuredSqlRulesDirectory = rulesEngineConfigurationValues.SqlRulesFileFolder;
            if (configuredSqlRulesDirectory.StartsWith("~"))
            {
                rulesEngineConfigurationValues.SqlRulesFileFolder = Server.MapPath(configuredSqlRulesDirectory);
            }

            var engineSchemaProvider = new EngineSchemaProvider(rulesEngineConfigurationValues);

            Func<Model> modelCreatorDelegate =
                () => new ModelBuilder(
                    new DirectoryRulesStreams(asConfiguredRulesDirectory).Streams)
                    .Build(null, engineSchemaProvider);

            container.Register(modelCreatorDelegate, Lifestyle.Scoped);
            container.Register<IOdsDataService, OdsDataService>(Lifestyle.Scoped);

            // Utility
            var loggerObj = LogManager.GetLogger(LoggerName);
            container.RegisterInstance(loggerObj);
            container.Register<ILoggingService, LoggingService>(Lifestyle.Singleton);
        }

        protected virtual void ConfigureWebApi(HttpConfiguration config)
        {
            // Creates the Dependency Injection Container, registers implementations, 
            // and uses it for all Web API object factories (e.g. IControllerFactory.CreateController).
            // Registers an instance of SimpleInjector.Container to resolve dependencies for Web API, and returns the instance.
            var container = new Container();

            // enable property injection for filter attributes
            // see https://simpleinjector.readthedocs.io/en/2.8/registermvcattributefilterprovider-is-deprecated.html
            container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            // Make the objects per-incoming-Web-Request by default.
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Web API Controller instances will be created by dependency injection.
            container.RegisterWebApiControllers(config);

            // Web API will use this DI container to create everything the framework knows how to inject.
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // Usually you'd get an HttpRequestMessage from the HttpContext class in System.Web, but Web API doesn't need System.Web.
            // So we'll let Simple Injector give us access when needed.
            container.EnableHttpRequestMessageTracking(config);

            // Allow getting the Request from arbitrary code 
            container.RegisterInstance<IRequestMessageAccessor>(new RequestMessageAccessor(container));

            container.RegisterMvcIntegratedFilterProvider();

            AddAppServicesToContainer(container);

            // Register a Custom Model Binder.
            var provider = new SimpleModelBinderProvider(typeof(ValidationErrorFilter), new ValidationErrorFilterModelBinder());
            config.Services.Insert(typeof(ModelBinderProvider), 0, provider);

            // Web API will use this DI container to create everything the framework knows how to inject.
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            AddWebApiRoutes(config);
            AddResponseFormatters(config);
            AddFilters(config, container);
            config.SuppressHostPrincipal();

            container.Verify();
        }

        protected virtual void AddWebApiRoutes(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();
        }

        protected virtual void AddResponseFormatters(HttpConfiguration httpConfiguration)
        {
            // This formatter uses Newtonsoft JSON.Net and handles Camel Casing, returns Enums as Strings, and converts the date into the ISO format JavaScript loves.
            httpConfiguration.Formatters.Insert(0, new JsonNetFormatter());

            // Hardcode all responses to use JSON content type (format) by removing XML support
            httpConfiguration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        }

        protected virtual void AddFilters(HttpConfiguration httpConfiguration, Container container)
        {
            httpConfiguration.Filters.Add(new ProfilingFilter(container.GetInstance<ILoggingService>()));
            httpConfiguration.Filters.Add(new ValidationWebExceptionFilterAttribute(container.GetInstance<IRequestMessageAccessor>()));

            // this got changed to use the service locator pattern
            httpConfiguration.Filters.Add(new ValidationAuthenticationFilter());

            // Unless marked with AllowAnonymous, all actions require Authentication.
            httpConfiguration.Filters.Add(new System.Web.Http.AuthorizeAttribute());
            httpConfiguration.MessageHandlers.Add(new LoggingHandler(container.GetInstance<ILoggingService>()));

            httpConfiguration.Services.Add(
                typeof(IExceptionLogger),
                new ValidationExceptionLogger(
                    container.GetInstance<ILoggingService>(),
                    container.GetInstance<IRequestMessageAccessor>()));
        }

        protected void Session_Start()
        {
        }

        #region Require Session for Web API, for security and authentication checks.
        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null &&
                   HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(ApiUrlPrefixRelative);
        }
        #endregion Require Session for Web API
    }
}