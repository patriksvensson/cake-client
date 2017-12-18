using Autofac;
using Cake.Commands;
using Cake.Common.Modules;
using Cake.Composition;
using Cake.Converters;
using Cake.Core;
using Cake.Core.Modules;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.Reflection;
using Cake.Modules;
using Cake.Temp;
using Spectre.CommandLine;

namespace Cake
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Create the command application.
            var app = new CommandApp(CreateTypeRegistrar());
            app.Configure(config =>
            {
                config.SetApplicationName("cake");

                config.AddCommand<BuildCommand>("build");
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
            
            registrar.RegisterType<Bootstrapper>().Singleton();
            registrar.RegisterType<ModuleSearcher>().Singleton();
            registrar.RegisterType<ModuleLoader>().Singleton();
            registrar.RegisterType<CakeConfigurationProvider>().Singleton();

            registrar.RegisterType<FilePathConverter>().Singleton();
            registrar.RegisterType<VerbosityConverter>().Singleton();

            // Temporary. Remove this later on.
            registrar.RegisterType<AssemblyLoader>().As<IAssemblyLoader>().Singleton();

            return new AutofacTypeRegistrar(registrar.Builder);
        }
    }
}
