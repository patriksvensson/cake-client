using Autofac;
using Cake.Common.Modules;
using Cake.Composition;
using Cake.Converters;
using Cake.Core;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Modules;
using Cake.Core.Scripting;
using Cake.Core.Scripting.Analysis;
using Cake.Modules;
using Cake.NuGet;
using Spectre.Cli;
using System;
using System.ComponentModel;
using System.Linq;
using IContainer = Autofac.IContainer;

namespace Cake.Commands
{
    [Description("Bootstraps the build.")]
    public sealed class BoostrapCommand : Command<BoostrapCommand.Settings>
    {
        private readonly Bootstrapper _bootstrapper;
        private readonly ICakeEnvironment _environment;

        public sealed class Settings : CommandSettings
        {
            [Description("The Cake script to bootstrap.")]
            [CommandArgument(0, "[SCRIPT]")]
            [DefaultValue("build.cake")]
            [TypeConverter(typeof(FilePathConverter))]
            public FilePath Script { get; set; }

            [Description("Specifies the amount of information to be displayed.")]
            [CommandOption("-v|--verbosity <VERBOSITY>")]
            [DefaultValue(Verbosity.Normal)]
            [TypeConverter(typeof(VerbosityConverter))]
            public Verbosity Verbosity { get; set; }
        }

        public BoostrapCommand(
            Bootstrapper bootstrapper,
            ICakeEnvironment environment)
        {
            _bootstrapper = bootstrapper;
            _environment = environment;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            // Get the script path and the root.
            var path = (settings.Script ?? new FilePath("build.cake")).MakeAbsolute(_environment);
            var root = path.GetDirectory();

            // Get the configuration.
            var configuration = _bootstrapper.GetConfiguration(root, context.Remaining.Parsed);

            // Create a completely new lifetime scope.
            using (var container = CreateLifetimeScope(settings, context.Remaining.Parsed, configuration))
            {
                // 
                var analyzer = container.Resolve<IScriptAnalyzer>();
                var processor = container.Resolve<IScriptProcessor>();

                // Analyze the script.
                var result = analyzer.Analyze(path);
                if (!result.Succeeded)
                {
                    var messages = string.Join("\n", result.Errors.Select(s => $"{root.GetRelativePath(s.File).FullPath}, line #{s.Line}: {s.Message}"));
                    throw new AggregateException($"Bootstrapping failed for '{path}'.\n{messages}");
                }

                // Install modules.
                processor.InstallModules(
                    result.Modules,
                    configuration.GetModulePath(root, _environment));
            }

            // Success.
            return 0;
        }

        private static IContainer CreateLifetimeScope(
            Settings settings,
            ILookup<string, string> remaining,
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
            registrar.RegisterModule(new ScriptingModule(false));

            // Misc registrations.
            registrar.RegisterType<CakeReportPrinter>().As<ICakeReportPrinter>().Singleton();
            registrar.RegisterType<CakeConsole>().As<IConsole>().Singleton();
            registrar.RegisterInstance(configuration).As<ICakeConfiguration>().Singleton();

            // Build the registrar.
            return registrar.Build();
        }
    }
}
