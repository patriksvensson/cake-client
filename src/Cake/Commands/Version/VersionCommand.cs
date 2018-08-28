using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Cake.Core;
using Spectre.Cli;

namespace Cake.Commands.Version
{
    [Description("Shows version information.")]
    internal sealed class VersionCommand : Command<VersionCommandSettings>
    {
        private readonly IConsole _console;

        public VersionCommand(IConsole console)
        {
            _console = console;
        }

        public override int Execute(CommandContext context, VersionCommandSettings settings)
        {
            var (version, product) = GetVersion();

            if (settings.Pretty)
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

                _console.ForegroundColor = System.ConsoleColor.Yellow;
                _console.WriteLine(@"   ___      _          ___       _ _     _ ");
                _console.WriteLine(@"  / __\__ _| | _____  / __\_   _(_) | __| |");
                _console.WriteLine(@" / /  / _` | |/ / _ \/__\// | | | | |/ _` |");
                _console.WriteLine(@"/ /___ (_| |   <  __/ \/  \ |_| | | | (_| |");
                _console.WriteLine(@"\____/\__,_|_|\_\___\_____/\__,_|_|_|\__,_|");
                _console.ResetColor();

                _console.WriteLine();
                _console.WriteLine(@"Version: {0}", version);
                _console.WriteLine(@"Details: {0}", string.Join("\n         ", product.Split('/')));
                _console.WriteLine();
            }
            else
            {
                _console.WriteLine(version);
            }

            return 0;
        }

        private static (string version, string product) GetVersion()
        {
            var assembly = typeof(VersionCommand).GetTypeInfo().Assembly;
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            return (info.FileVersion.ToString(), info.ProductVersion);
        }
    }
}