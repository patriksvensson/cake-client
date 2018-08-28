using System.ComponentModel;
using Spectre.Cli;

namespace Cake.Commands.Run
{
    public sealed class RunCommandSettings : ExecutableCommandSettings
    {
        [Description("Performs a debug.")]
        [CommandOption("-d|--debug")]
        public bool Debug { get; set; }

        [Description("Performs a dry run.")]
        [CommandOption("--dryrun")]
        public bool Dryrun { get; set; }

        [Description("Shows description about tasks.")]
        [CommandOption("--showdescription")]
        public bool ShowDescriptions { get; set; }

        [Description("Execute a single task without any dependencies.")]
        [CommandOption("--exclusive")]
        public bool Exclusive { get; set; }
    }
}