using Autofac;
using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.IO;
using Cake.Core.Scripting;
using Cake.Core.Scripting.Analysis;
using Spectre.Cli;
using System;
using System.ComponentModel;
using System.Linq;

namespace Cake.Commands.Bootstrap
{
    [Description("Bootstraps the build.")]
    public sealed class BoostrapCommand : ExecutableCommand<BootstrapCommandSettings>
    {
        private readonly ICakeEnvironment _environment;

        public BoostrapCommand(IFileSystem fileSystem, ICakeEnvironment environment)
            : base(fileSystem, environment)
        {
            _environment = environment;
        }

        public override int Execute(CommandContext context, ICakeConfiguration configuration, BootstrapCommandSettings settings)
        {
            var path = settings.Script;
            var root = path.GetDirectory();

            using (var container = CreateScope(settings, context, configuration))
            {
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

            return 0;
        }
    }
}
