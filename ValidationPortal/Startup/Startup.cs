using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using log4net.Config;
using Microsoft.Owin;
using Owin;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SimpleInjector.Integration.WebApi;
using System.Diagnostics;
using System.Web.Http.ExceptionHandling;
using System.Net.Http;
using log4net;

[assembly: OwinStartup(typeof(MDE.ValidationPortal.Startup))]

namespace MDE.ValidationPortal
{
    public class Startup
    {
        // (When run as a Windows Service - disregard when hosted in IIS .)
        // 1. This Startup class is specified as a type parameter in the WebApp.Start method Program.cs:
        //    WebApp.Start<Startup>("http://*:6902");   
        // 2. Command Prompt => Run as Administrator =>
        //    netsh http add urlacl http://*:6902/ user=EVERYONE
        // 3. On Windows Firewall add an Inbound Rule that opens TCP port 6902

        private static string[] _explicitlyRegisteredTypeNames = { "RequestMessageAccessor", "ApplicationLoggerService" };
        private const string LoggerName = "ValidationPortalLogger";

        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure logging
            ConfigureLogging();
            // Configure the Web API
            var httpConfiguration = CreateCustomHttpConfiguration();
            appBuilder.UseWebApi(httpConfiguration);
        }

        /// <summary>
        /// Writes log to C:\ProgramData\ValidationEngine\ValidationPortal.log - or wherever Program Data environmental variable points to.
        /// </summary>
        protected virtual void ConfigureLogging()
        {
            XmlConfigurator.Configure();
        }

        protected virtual HttpConfiguration CreateCustomHttpConfiguration()
        {
            // This code would go normally go in App_Start\WebApiConfig, called from Global.asax. 
            HttpConfiguration httpConfiguration = new HttpConfiguration();
            // Creates the Dependency Injection Container, registers implementations, 
            // and uses it for all Web API object factories (e.g. IControllerFactory.CreateController).
            var container = AddDependencyResolver(httpConfiguration);
            AddWebApiRoutes(httpConfiguration);
            AddResponseFormatters(httpConfiguration);
            AddFilters(httpConfiguration, container);
            return httpConfiguration;
        }

        protected virtual void AddWebApiRoutes(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();

            // Default to Version 1.1 of the OneRoster standard.
            httpConfiguration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
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
            httpConfiguration.Filters.Add(new ProfilingFilter(container.GetInstance<IApplicationLoggerService>()));
            // Unless marked with AllowAnonymous, all actions require Authenication.
            httpConfiguration.Filters.Add(new AuthorizeAttribute());
            httpConfiguration.Filters.Add(new CustomExceptionFilterAttribute(container.GetInstance<IRequestMessageAccessor>()));
            httpConfiguration.Filters.Add(new CustomAuthenticationFilter(httpConfiguration, container.GetInstance<IApplicationLoggerService>()));
            httpConfiguration.MessageHandlers.Add(new LoggingHandler(container.GetInstance<IApplicationLoggerService>()));
            // httpConfiguration.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            httpConfiguration.Services.Add(typeof(IExceptionLogger), new ExporterExceptionLogger(container.GetInstance<IApplicationLoggerService>(), container.GetInstance<IRequestMessageAccessor>()));
        }

        protected virtual Container AddDependencyResolver(HttpConfiguration httpConfiguration)
        {
            // Registers an instance of SimpleInjector.Container to resolve dependencies for Web API, and returns the instance.
            var container = CreateDependencyInjectionContainer(httpConfiguration);

            // Usually you'd get an HttpRequestMessage from teh HttpContext class in System.Web, but Web API doesn't need System.Web.
            // So we'll let Simple Injector give us access when needed.
            container.EnableHttpRequestMessageTracking(httpConfiguration);

            // Dynamically (late binding) register any implementations of similarly named interfaces (Something : ISomething).
            // For service assemblies
            var serviceAssemblies = GetServiceAssemblies();
            foreach (var serviceAssembly in serviceAssemblies)
            {
                RegisterTypesByConvention(container, serviceAssembly);
                RegisterDbContexts(container, serviceAssembly);
            }

            // Logging
            RegisterLogger(container);

            container.Verify();

            return container;
        }

        protected virtual Container CreateDependencyInjectionContainer(HttpConfiguration httpConfiguration)
        {
            var container = new Container();

            // Make the objects per-incoming-Web-Request by default.
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Web API Controller instances will be created by dependency injection.
            container.RegisterWebApiControllers(httpConfiguration);

            // Web API will use this DI container to create everything the framework knows how to inject.
            httpConfiguration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            return container;
        }

        protected virtual IEnumerable<Assembly> GetServiceAssemblies()
        {
            // For a Windows Service
            // var rootDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            // For a Web App
            var binDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));
            var codeLibraries = binDirectory.GetFiles().Where(file => file.Extension.ToLower().EndsWith("dll"))
                .Select(f => Assembly.Load(AssemblyName.GetAssemblyName(f.FullName)));
            var serviceLibraries = codeLibraries.Where(lib => lib.GetTypes().Any(
                typ => typ.Namespace != null && typ.Namespace.Contains("DoubleLine"))
                ).ToArray();
            return serviceLibraries;
        }

        /// <summary>
        /// Adds (to the DI container) any Types that implement an Interface of the same name:  ISomething => Something.
        /// Excludes the ODS SDK, which doesn't use DI/IoC internally.
        /// </summary>
        protected virtual void RegisterTypesByConvention(Container container, Assembly assembly)
        {
            var implementations = assembly.GetTypes()
                .SelectMany(t => t.GetInterfaces(), (ty, iface) => new {
                    TypeName = ty.Name,
                    InterfaceName = iface.Name,
                    TheType = ty,
                    TheInterface = iface
                })
                .Where(pair => string.Compare("I" + pair.TypeName, pair.InterfaceName, true) == 0 && !pair.TheInterface.Namespace.Contains("Ods.Sdk"))
                .ToArray();
            foreach (var impl in implementations)
            {
                Debug.WriteLine(string.Format("{0} {1}", impl.InterfaceName, impl.TypeName));
                if (!_explicitlyRegisteredTypeNames.Contains(impl.TheType.Name))
                {
                    container.Register(impl.TheInterface, impl.TheType, Lifestyle.Scoped);
                }
            }
        }

        /// <summary>
        /// Adds (to the DI container) any Types that implement an IDbContext with their Concrete selfs, 
        /// except EdFi contexts, which are created with a Factory so that the database accessed can vary by requestor.
        /// </summary>
        protected virtual void RegisterDbContexts(Container container, Assembly assembly)
        {
            var implementations = assembly.GetTypes()
                .Where(typeX => typeX.BaseType != null && typeX.BaseType.Name.EndsWith("DbContext") && !typeX.Name.Contains("EdFi"))
                .Select(t => new {
                    TypeName = t.Name,
                    BaseTypeName = t.BaseType.Name,
                    TheType = t,
                    TheBaseType = t.BaseType
                })
                .ToArray();
            foreach (var impl in implementations)
            {
                Debug.WriteLine(string.Format("{0} {0}", impl.TypeName));
                container.Register(impl.TheType, impl.TheType, Lifestyle.Scoped);
            }
        }

        protected virtual void RegisterLogger(Container container)
        {
            container.RegisterInstance<IRequestMessageAccessor>(new RequestMessageAccessor(container));
            container.RegisterInstance<IApplicationLoggerService>(new ApplicationLoggerService(LogManager.GetLogger(LoggerName), new RequestMessageAccessor(container)));
        }
    }
}
