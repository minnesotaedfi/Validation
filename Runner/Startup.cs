using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using CommandLine;
using Engine.Language;
using Engine.Utility;
using log4net.Config;

[assembly: OwinStartup(typeof(Runner.Startup))]

namespace Runner
{
    public class Startup
    {
        private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var parser = new Parser(p =>
            {
                p.HelpWriter = Console.Error;
            });
            var parseResult = parser.ParseArguments<Options>(args);
            var errors = parseResult.Errors.ToArray();
            if (errors.Length > 0)
            {
                Environment.ExitCode = 1;
                return;
            }
            if (parseResult.Value.Web)
                StartServer(parseResult.Value);
            else
                if (!(RunRules(parseResult.Value)))
                {
                    Environment.ExitCode = 1;
                    return;
                }
            return;
        }

        private static bool RunRules(Options options)
        {
            var container = BuildCompositeRoot(options);
            var runner = container.Resolve<RuleRunner>();
            if (options.Test)
                return runner.Test(options.ConnectionString, options.Collection).GetAwaiter().GetResult();

            else
                return runner.Run(options.ConnectionString, options.Collection).GetAwaiter().GetResult();
        }

        private static void StartServer(Options options)
        {
            string baseAddress = $"http://localhost:{options.Port}/";
            using (WebApp.Start(baseAddress, app =>
            {
                var container = BuildCompositeRoot(options, true);
                app.UseAutofacMiddleware(container);

                var config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();
                app.UseAutofacWebApi(config);
                app.UseWebApi(config);

                app.MapSignalR();

                var fileServerOptions = new FileServerOptions
                {
                    EnableDirectoryBrowsing = false,
                    EnableDefaultFiles = true,
                    FileSystem = new EmbeddedResourceFileSystem("Runner")
                };
                app.UseFileServer(fileServerOptions);
            }))
            {
                Process.Start(baseAddress);
                Console.WriteLine($"Started on {baseAddress}...");
                Console.ReadKey();
                TokenSource.Cancel();
            }
        }

        private static IContainer BuildCompositeRoot(Options options, bool isWeb = false)
        {
            var builder = new ContainerBuilder();

            if (isWeb)
            {
                options.RulesPath = "./Rules";

                // ApiController
                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            }
            // ISchemaProvider
            builder.RegisterType<SettingsSchemaProvider>().As<ISchemaProvider>().SingleInstance();

            // Options, ISchemaProvider, IConstantValueProvider
            builder.Register(c => options);
            builder.Register(c => options).As<IConstantValueProvider>();
            builder.Register(c => options).As<ISchemaProvider>();

            // IConstantValueProvider
            builder.RegisterType<SettingsConstantValueProvider>().As<IConstantValueProvider>();

            // IRulesStreamsProvider
            builder.RegisterType<ConstantsRulesStreams>().As<IRulesStreamsProvider>();
            builder.Register<IRulesStreamsProvider>(c => new DirectoryRulesStreams(options.RulesPath));

            // Stream[]
            builder.Register(c => c.Resolve<IEnumerable<IRulesStreamsProvider>>().SelectMany(x => x.Streams).ToArray());

            // ModelBuilder
            builder.RegisterType<ModelBuilder>();

            // Model
            builder.Register(c => c.Resolve<ModelBuilder>().Build(schemaProvider: c.Resolve<ISchemaProvider>())).SingleInstance();

            // RuleRunner
            builder.RegisterType<RuleRunner>().SingleInstance();

            return builder.Build();
        }
    }
}
