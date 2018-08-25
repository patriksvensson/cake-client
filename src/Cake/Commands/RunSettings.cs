using System.ComponentModel;
using Cake.Converters;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Spectre.Cli;

namespace Cake.Commands
{
    public sealed class RunSettings : CommandSettings
    {
        [Description("The Cake script to execute.")]
        [CommandArgument(0, "[SCRIPT]")]
        [DefaultValue("build.cake")]
        [TypeConverter(typeof(FilePathConverter))]
        public FilePath Script { get; set; }

        [Description("Specifies the amount of information to be displayed.")]
        [CommandOption("-v|--verbosity <VERBOSITY>")]
        [DefaultValue(Verbosity.Normal)]
        [TypeConverter(typeof(VerbosityConverter))]
        public Verbosity Verbosity { get; set; }

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