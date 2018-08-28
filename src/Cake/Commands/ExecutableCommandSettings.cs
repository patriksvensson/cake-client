using Cake.Converters;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Spectre.Cli;
using System.ComponentModel;

namespace Cake.Commands
{
    public abstract class ExecutableCommandSettings : CommandSettings
    {
        [Description("The Cake script.")]
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
}
