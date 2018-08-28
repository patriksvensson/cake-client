using Autofac;
using Cake.Commands;
using Cake.Composition;
using Cake.Converters;
using Cake.Core;
using Cake.Core.Modules;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Spectre.Cli;
using Cake.Utilities;
using Cake.Commands.Run;
using Cake.Commands.Bootstrap;
using Cake.Commands.Version;

namespace Cake
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp<RunCommand>(CreateTypeRegistrar());
            app.Configure(config =>
            {
                config.SetApplicationName("cake");

                config.AddCommand<RunCommand>("run");
                config.AddCommand<BoostrapCommand>("bootstrap");
                config.AddCommand<VersionCommand>("version");
            });

            // Run the application.
            return app.Run(args);
        }

        private static ITypeRegistrar CreateTypeRegistrar()
        {
            var registrar = new ContainerRegistrar();

            registrar.RegisterModule(new CoreModule());
            registrar.RegisterType<CakeConsole>().As<IConsole>().Singleton();
            registrar.RegisterType<CakeBuildLog>().As<ICakeLog>().Singleton();

            registrar.RegisterType<ConfigReader>().Singleton();
            registrar.RegisterType<ModuleSearcher>().Singleton();
            registrar.RegisterType<ModuleLoader>().Singleton();
            registrar.RegisterType<CakeConfigurationProvider>().Singleton();

            registrar.RegisterType<FilePathConverter>().Singleton();
            registrar.RegisterType<VerbosityConverter>().Singleton();

            return new AutofacTypeRegistrar(registrar.Builder);
        }
    }
}
