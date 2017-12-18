using Cake.Core.Diagnostics;

namespace Cake.Diagnostics
{
    internal sealed class ConsoleLogSettings
    {
        public Verbosity Verbosity { get; }

        public ConsoleLogSettings(Verbosity verbosity)
        {
            Verbosity = verbosity;
        }
    }
}