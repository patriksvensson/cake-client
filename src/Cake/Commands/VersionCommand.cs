using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cake.Core;
using Spectre.CommandLine;

namespace Cake.Commands
{
    [Description("Shows version information.")]
    internal sealed class VersionCommand : Command<VersionCommand.Settings>
    {
        private readonly IConsole _console;

        public sealed class Settings
        {
        }

        public VersionCommand(IConsole console)
        {
            _console = console;
        }

        public override int Execute(Settings settings, ILookup<string, string> remaining)
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

            return 0;
        }

        private static string GetVersion()
        {
            var assembly = typeof(VersionCommand).GetTypeInfo().Assembly;
            return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        }
    }
}