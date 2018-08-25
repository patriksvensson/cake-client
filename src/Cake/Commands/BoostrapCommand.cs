using Spectre.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Commands
{
    [Description("Bootstraps the build.")]
    public sealed class BootstrapCommand : Command<BootstrapCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            return 0;
        }
    }
}
