using System.ComponentModel;
using System.Linq;
using Autofac;
using Cake.Common.Modules;
using Cake.Composition;
using Cake.Core;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Modules;
using Cake.Core.Scripting;
using Cake.Diagnostics;
using Cake.Modules;
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
        private readonly Bootstrapper _bootstrapper;
        private readonly ICakeEnvironment _environment;

        public BuildCommand(Bootstrapper bootstrapper, ICakeEnvironment environment)
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

            // Create a completely new lifetime scope.
            using (var container = CreateLifetimeScope(settings, remaining, configuration))
            {
                var runner = container.Resolve<IScriptRunner>();
                var host = BuildScriptHost(settings, container);

                runner.Run(host, settings.Script, new CakeArguments(remaining).Arguments);
            }

            return 0;
        }

        private static IContainer CreateLifetimeScope(BuildSettings settings, ILookup<string, string> remaining, ICakeConfiguration configuration)
        {
            var registrar = new ContainerRegistrar();

            // External modules
            registrar.RegisterModule(new CoreModule());
            registrar.RegisterModule(new CommonModule());
            registrar.RegisterModule(new NuGetModule(configuration));

            // Internal modules.
            registrar.RegisterModule(new LoggingModule(settings.Verbosity));
            registrar.RegisterModule(new ArgumentsModule(remaining));
            registrar.RegisterModule(new ScriptingModule(settings.Debug));

            // Misc registrations.
            registrar.RegisterType<CakeReportPrinter>().As<ICakeReportPrinter>().Singleton();
            registrar.RegisterType<CakeConsole>().As<IConsole>().Singleton();
            registrar.RegisterInstance(configuration).As<ICakeConfiguration>().Singleton();

            return registrar.Build();
        }

        private static IScriptHost BuildScriptHost(BuildSettings settings, IContainer container)
        {
            if (settings.Dryrun)
            {
                return container.Resolve<DryRunScriptHost>();
            }
            return container.Resolve<BuildScriptHost>();
        }
    }
}
