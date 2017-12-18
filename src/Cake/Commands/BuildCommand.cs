using System.ComponentModel;
using System.Linq;
using Autofac;
using Cake.Common.Modules;
using Cake.Core;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Modules;
using Cake.Core.Scripting;
using Cake.Internal;
using Cake.Internal.Composition;
using Cake.Internal.Diagnostics;
using Cake.NuGet;
using Cake.Scripting;
using Cake.Scripting.Roslyn;
using Spectre.CommandLine;
using IContainer = Autofac.IContainer;

namespace Cake.Commands
{
    [Description("Runs the build.")]
    public sealed class BuildCommand : Command<BuildSettings>
    {
        private readonly IBootstrapper _bootstrapper;
        private readonly ICakeEnvironment _environment;

        public BuildCommand(IBootstrapper bootstrapper, ICakeEnvironment environment)
        {
            _bootstrapper = bootstrapper;
            _environment = environment;
        }

        public override int Execute(BuildSettings settings, ILookup<string, string> remaining)
        {
            // Fix the script path.
            settings.Script = settings.Script ?? new FilePath("build.cake");
            settings.Script = settings.Script.MakeAbsolute(_environment);

            // Get the configuration and all modules.
            var configuration = _bootstrapper.GetConfiguration(settings, remaining);
            var modules = _bootstrapper.LoadModules(settings, configuration);

            // Create a new container.
            using (var container = CreateRegistrar(settings, remaining, configuration))
            {
                var runner = container.Resolve<IScriptRunner>();
                var host = container.Resolve<BuildScriptHost>();

                runner.Run(host, settings.Script, new CakeArguments(remaining).Arguments);
            }

            // Now we build up the new container.
            return 0;
        }

        private static IContainer CreateRegistrar(BuildSettings settings, ILookup<string, string> remaining, ICakeConfiguration configuration)
        {
            var registrar = new ContainerRegistrar();

            // Modules
            registrar.RegisterModule(new CoreModule());
            registrar.RegisterModule(new CommonModule());
            registrar.RegisterModule(new NuGetModule(configuration));

            // Core
            registrar.RegisterType<CakeReportPrinter>().As<ICakeReportPrinter>().Singleton();
            registrar.RegisterType<CakeConsole>().As<IConsole>().Singleton();
            registrar.RegisterInstance(configuration).As<ICakeConfiguration>().Singleton();

            // Scripting
            registrar.RegisterType<RoslynScriptEngine>().As<IScriptEngine>().Singleton();
            registrar.RegisterType<BuildScriptHost>().Singleton();
            registrar.RegisterType<DescriptionScriptHost>().Singleton();
            registrar.RegisterType<DryRunScriptHost>().Singleton();

            // Cake
            registrar.RegisterType<CakeLog>().As<ICakeLog>().Singleton();
            registrar.RegisterInstance(new LogSettings(settings.Verbosity)).AsSelf().Singleton();
            registrar.RegisterInstance(new CakeArguments(remaining)).As<ICakeArguments>().Singleton();

            var scriptOptions = new ScriptOptions();
            if (settings.Debug)
            {
                scriptOptions.PerformDebug = true;
            }
            registrar.RegisterInstance(scriptOptions).As<ScriptOptions>().Singleton();

            return registrar.Build();
        }
    }
}
