using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Cake.Core;
using Spectre.Cli;

namespace Cake.Commands
{
    [Description("Shows version information.")]
    internal sealed class VersionCommand : Command<VersionCommand.Settings>
    {
        private readonly IConsole _console;

        public sealed class Settings : CommandSettings
        {
        }

        public VersionCommand(IConsole console)
        {
            _console = console;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            _console.WriteLine();
            _console.WriteLine(@"             +##   #;;'");
            _console.WriteLine(@"             #;;#  .+;;;;+,");
            _console.WriteLine(@"             '+;;#;,+';;;;;'#.");
            _console.WriteLine(@"             ++'''';;;;;;;;;;# ;#;");
            _console.WriteLine(@"            ##';;;;++'+#;;;;;'.   `#:");
            _console.WriteLine(@"         ;#   '+'';;;;;;;;;'#`       #.");
            _console.WriteLine(@"      `#,        .'++;;;;;':..........#");
            _console.WriteLine(@"    '+      `.........';;;;':.........#");
            _console.WriteLine(@"   #..................+;;;;;':........#");
            _console.WriteLine(@"   #..................#';;;;;'+''''''.#");
            _console.WriteLine(@"   #.......,:;''''''''##';;;;;'+'''''#,");
            _console.WriteLine(@"   #''''''''''''''''''###';;;;;;+''''#");
            _console.WriteLine(@"   #''''''''''''''''''####';;;;;;#'''#");
            _console.WriteLine(@"   #''''''''''''''''''#####';;;;;;#''#");
            _console.WriteLine(@"   #''''''''''''''''''######';;;;;;#'#");
            _console.WriteLine(@"   #''''''''''''''''''#######';;;;;;##");
            _console.WriteLine(@"   #''''''''''''''''''########';;;;;;#");
            _console.WriteLine(@"   #''''''''''''++####+;#######';;;;;;#");
            _console.WriteLine(@"   #+####':,`             ,#####';;;;;;'");
            _console.WriteLine(@"                              +##'''''+.");
            _console.WriteLine(@"   ___      _          ___       _ _     _ ");
            _console.WriteLine(@"  / __\__ _| | _____  / __\_   _(_) | __| |");
            _console.WriteLine(@" / /  / _` | |/ / _ \/__\// | | | | |/ _` |");
            _console.WriteLine(@"/ /___ (_| |   <  __/ \/  \ |_| | | | (_| |");
            _console.WriteLine(@"\____/\__,_|_|\_\___\_____/\__,_|_|_|\__,_|");
            _console.WriteLine();
            _console.WriteLine(@"                             Version {0}", GetVersion());
            _console.WriteLine();

            return 0;
        }

        private static string GetVersion()
        {
            var assembly = typeof(VersionCommand).GetTypeInfo().Assembly;
            return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        }
    }
}