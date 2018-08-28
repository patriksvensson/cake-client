using Autofac;
using Cake.Core;
using Cake.Core.Composition;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Packaging;
using Cake.Core.Scripting;
using Cake.Core.Scripting.Analysis;
using Cake.Scripting;
using Cake.Utilities;
using Spectre.Cli;
using System;
using System.Linq;
using System.ComponentModel;

namespace Cake.Commands.Run
{
    [Description("Runs the build.")]
    public sealed class RunCommand : ExecutableCommand<RunCommandSettings>
    {
        private readonly ICakeEnvironment _environment;
        private readonly ModuleLoader _loader;

        public RunCommand(IFileSystem fileSystem, ICakeEnvironment environment, ModuleLoader loader)
            : base(fileSystem, environment)
        {
            _environment = environment;
            _loader = loader;
        }

        public override int Execute(CommandContext context, ICakeConfiguration configuration, RunCommandSettings settings)
        {
            void ConfigureModules(ICakeContainerRegistrar registrar)
            {
                using (var scope = CreateScope(settings, context, configuration))
                {
                    var analyzer = scope.Resolve<IScriptAnalyzer>();
                    var installer = scope.Resolve<IPackageInstaller>();
                    var result = analyzer.Analyze(settings.Script);
                    if (result.Modules.Count > 0)
                    {
                        var root = GetModulePath(configuration, settings.Script.GetDirectory());
                        foreach (var item in result.Modules)
                        {
                            if (installer.CanInstall(item, PackageType.Module))
                            {
                                var files = installer.Install(item, PackageType.Module, root);
                                foreach (var file in files.Where(x => x.Path.GetExtension() == ".dll"))
                                {
                                    var module = _loader.LoadModule(scope, file.Path);
                                    if (module != null)
                                    {
                                        module.Register(registrar);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            using (var scope = CreateScope(settings, context, configuration, ConfigureModules))
            {
                var runner = scope.Resolve<IScriptRunner>();
                var log = scope.Resolve<ICakeLog>();

                var args = new CakeArguments(context.Remaining).Arguments;

                // Create the script host.
                var host = CreateScriptHost(settings, scope);
                if (settings.Exclusive)
                {
                    host.Settings.UseExclusiveTarget();
                }

                try
                {
                    // Run the script.
                    runner.Run(host, settings.Script, args);
                }
                catch (Exception ex)
                {
                    LogException(log, ex);
                    return 1;
                }
            }

            return 0;
        }

        private DirectoryPath GetModulePath(ICakeConfiguration configuration, DirectoryPath root)
        {
            var modulePath = configuration.GetValue("Paths_Modules");
            if (!string.IsNullOrWhiteSpace(modulePath))
            {
                return new DirectoryPath(modulePath).MakeAbsolute(_environment);
            }
            var toolPath = GetToolPath(configuration, root);
            return toolPath.Combine("Modules").Collapse();
        }

        private DirectoryPath GetToolPath(ICakeConfiguration configuration, DirectoryPath root)
        {
            var toolPath = configuration.GetValue("Paths_Tools");
            if (!string.IsNullOrWhiteSpace(toolPath))
            {
                return new DirectoryPath(toolPath).MakeAbsolute(_environment);
            }
            return root.Combine("tools");
        }

        private static ScriptHost CreateScriptHost(RunCommandSettings settings, ILifetimeScope container)
        {
            if (settings.Dryrun)
            {
                return container.Resolve<DryRunScriptHost>();
            }
            if (settings.ShowDescriptions)
            {
                return container.Resolve<DescriptionScriptHost>();
            }
            return container.Resolve<BuildScriptHost>();
        }

        private static void LogException<T>(ICakeLog log, T ex) where T : Exception
        {
            log = log ?? new CakeBuildLog(new CakeConsole());
            if (log.Verbosity == Verbosity.Diagnostic)
            {
                log.Error("Error: {0}", ex);
            }
            else
            {
                log.Error("Error: {0}", ex.Message);
                if (ex is AggregateException aex)
                {
                    foreach (var exception in aex.InnerExceptions)
                    {
                        log.Error("\t{0}", exception.Message);
                    }
                }
            }
        }
    }
}
