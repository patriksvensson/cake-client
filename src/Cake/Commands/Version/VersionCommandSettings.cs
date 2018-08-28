using Spectre.Cli;
using System.ComponentModel;

namespace Cake.Commands.Version
{
    public sealed class VersionCommandSettings : CommandSettings
    {
        [Description("Shows pretty version information.")]
        [CommandOption("-p|--pretty")]
        public bool Pretty { get; set; }
    }
}