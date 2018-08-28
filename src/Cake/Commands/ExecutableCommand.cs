using Autofac;
using Cake.Common.Modules;
using Cake.Composition;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Composition;
using Cake.Core.IO;
using Cake.Core.Modules;
using Cake.Modules;
using Cake.NuGet;
using Spectre.Cli;
using System.Linq;
using System;

namespace Cake.Commands
{
    public abstract class ExecutableCommand<T> : Command<T>
        where T : ExecutableCommandSettings
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;

        protected ExecutableCommand(IFileSystem fileSystem, ICakeEnvironment environment)
        {
            _fileSystem = fileSystem;
            _environment = environment;
        }

        public sealed override int Execute(CommandContext context, T settings)
        {
            // Fix the script path.
            settings.Script = settings.Script ?? new FilePath("build.cake");
            settings.Script = settings.Script.MakeAbsolute(_environment);

            // Get the configuration and all modules.
            return Execute(context, ReadConfiguration(context, settings.Script), settings);
        }

        public abstract int Execute(CommandContext context, ICakeConfiguration configuration, T settings);

        protected IContainer CreateScope(
            T settings,
            CommandContext context,
            ICakeConfiguration configuration,
            Action<ICakeContainerRegistrar> registrations = null)
        {
            var registrar = new ContainerRegistrar();

            // External modules
            registrar.RegisterModule(new CoreModule());
            registrar.RegisterModule(new CommonModule());
            registrar.RegisterModule(new NuGetModule(configuration));

            // Internal modules.
            registrar.RegisterModule(new LoggingModule(settings.Verbosity));
            registrar.RegisterModule(new ArgumentsModule(context.Remaining));
            registrar.RegisterModule(new ScriptingModule(false));

            // Misc registrations.
            registrar.RegisterType<CakeReportPrinter>().As<ICakeReportPrinter>().Singleton();
            registrar.RegisterType<CakeConsole>().As<IConsole>().Singleton();
            registrar.RegisterInstance(configuration).As<ICakeConfiguration>().Singleton();

            // Register custom registrations.
            registrations?.Invoke(registrar);

            // Build the registrar.c
            return registrar.Build();
        }

        public ICakeConfiguration ReadConfiguration(CommandContext context, FilePath script)
        {
            var provider = new CakeConfigurationProvider(_fileSystem, _environment);
            var root = script.GetDirectory();
            var args = context.Remaining.Parsed.ToDictionary(x => x.Key, x => x.FirstOrDefault() ?? string.Empty);
            return provider.CreateConfiguration(root, args);
        }
    }
}
