using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Spectre.CommandLine;

namespace Cake.Commands
{
    [Description("Shows version information.")]
    internal sealed class VersionCommand : Command<VersionCommand.Settings>
    {
        public sealed class Settings
        {
        }

        public override int Execute(Settings settings, ILookup<string, string> remaining)
        {
            return 0;
        }
    }
}