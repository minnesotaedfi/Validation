using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Engine.Language;
using ValidationWeb.Services;
using Engine.Utility;
using System.IO;
using Engine.Models;

namespace ValidationWeb
{
    public class Global : System.Web.HttpApplication
    {
        private AppSettingsFileConfigurationValues config = new AppSettingsFileConfigurationValues();
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ConfigureDependencyInjection();
            GlobalFilters.Filters.Add(new PortalAuthenticationFilter());
        }

        protected void ConfigureDependencyInjection()
        {
            // Create a Simple Injector container, and register concrete instances.
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            // This is an extension method from the MVC integration package.
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            // Services registered will depend on whether in production or test mode.
            if (config.UseFakeViewModelData)
            {
                // Web Portal
                container.Register<IAnnouncementService, FakeAnnouncementService>(Lifestyle.Scoped);
                container.Register<IAppUserService, FakeAppUserService>(Lifestyle.Scoped);
                container.Register<IConfigurationValues, AppSettingsFileConfigurationValues>(Lifestyle.Scoped);
                container.Register<IEdOrgService, FakeEdOrgService>(Lifestyle.Scoped);
                container.Register<IHttpContextProvider, HttpContextProvider>(Lifestyle.Scoped);
                container.Register<ISchoolYearService, SchoolYearService>(Lifestyle.Scoped);
                container.Register<IValidatedDataSubmissionService, FakeValidatedDataSubmissionService>(Lifestyle.Scoped);
                container.Register<IValidationResultsService, FakeValidationResultsService>(Lifestyle.Scoped);

                // Entity Framework Database Contexts
                container.Register<ValidationPortalDbContext>(Lifestyle.Scoped);
                container.Register<IOdsConfigurationValues, OdsConfigurationValues>(Lifestyle.Scoped);

                // Rules Engine
                container.Register<ISchemaProvider, EngineSchemaProvider>(Lifestyle.Scoped);
                container.Register<IRulesEngineService, RulesEngineService>(Lifestyle.Scoped);
                container.Register<IRulesEngineConfigurationValues, RulesEngineConfigurationValues>(Lifestyle.Scoped);
                Func<Model> modelCreatorDelegate = () => new ModelBuilder(new DirectoryRulesStreams((new RulesEngineConfigurationValues().RulesFileFolder)).Streams).Build(null, new EngineSchemaProvider());
                container.Register<Model>(modelCreatorDelegate, Lifestyle.Scoped);
            }
            else
            {
                // Web Portal
                container.Register<IAnnouncementService, AnnouncementService>(Lifestyle.Scoped);
                container.Register<IAppUserService, AppUserService>(Lifestyle.Scoped);
                container.Register<IConfigurationValues, AppSettingsFileConfigurationValues>(Lifestyle.Scoped);
                container.Register<IEdOrgService, EdOrgService>(Lifestyle.Scoped);
                container.Register<IHttpContextProvider, HttpContextProvider>(Lifestyle.Scoped);
                container.Register<ISchoolYearService, SchoolYearService>(Lifestyle.Scoped);
                container.Register<IValidatedDataSubmissionService, ValidatedDataSubmissionService>(Lifestyle.Scoped);
                container.Register<IValidationResultsService, ValidationResultsService>(Lifestyle.Scoped);

                // Entity Framework Database Contexts
                container.Register<ValidationPortalDbContext>(Lifestyle.Scoped);
                container.Register<IOdsConfigurationValues, OdsConfigurationValues>(Lifestyle.Scoped);

                // Rules Engine
                container.Register<ISchemaProvider, EngineSchemaProvider>(Lifestyle.Scoped);
                container.Register<IRulesEngineService, RulesEngineService>(Lifestyle.Scoped);
                container.Register<IRulesEngineConfigurationValues, RulesEngineConfigurationValues>(Lifestyle.Scoped);
                Func<Model> modelCreatorDelegate = () => new ModelBuilder(new DirectoryRulesStreams((new RulesEngineConfigurationValues().RulesFileFolder)).Streams).Build(null, new EngineSchemaProvider());
                container.Register<Model>(modelCreatorDelegate, Lifestyle.Scoped);
            }

            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}