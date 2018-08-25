using System.Collections.Generic;
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
using Spectre.Cli;
using IContainer = Autofac.IContainer;

namespace Cake.Commands
{
    [Description("Runs the build.")]
    public sealed class RunCommand : Command<RunSettings>
    {
        private readonly Bootstrapper _bootstrapper;
        private readonly ICakeEnvironment _environment;

        public RunCommand(Bootstrapper bootstrapper, ICakeEnvironment environment)
        {
            _bootstrapper = bootstrapper;
            _environment = environment;
        }

        public override int Execute(CommandContext context, RunSettings settings)
        {
            // Fix the script path.
            settings.Script = settings.Script ?? new FilePath("build.cake");
            settings.Script = settings.Script.MakeAbsolute(_environment);

            // Get the configuration and all modules.
            var configuration = _bootstrapper.GetConfiguration(settings, context.Remaining.Parsed);
            var modules = _bootstrapper.LoadModules(settings, configuration);

            // Create a completely new lifetime scope.
            using (var container = CreateLifetimeScope(settings, context.Remaining.Parsed, modules, configuration))
            {
                var runner = container.Resolve<IScriptRunner>();
                var host = settings.Dryrun
                    ? (IScriptHost)container.Resolve<DryRunScriptHost>()
                    : container.Resolve<BuildScriptHost>();

                runner.Run(host, settings.Script, new CakeArguments(context.Remaining.Parsed).Arguments);
            }

            return 0;
        }

        private static IContainer CreateLifetimeScope(
            RunSettings settings, 
            ILookup<string, string> remaining,
            IEnumerable<ICakeModule> modules,
            ICakeConfiguration configuration)
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

            // Register modules.
            foreach(var module in modules)
            {
                module.Register(registrar);
            }

            // Build the registrar.
            return registrar.Build();
        }
    }
}
