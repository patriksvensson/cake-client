using Cake.Core.Diagnostics;

namespace Cake.Internal.Diagnostics
{
    internal sealed class LogSettings
    {
        public Verbosity Verbosity { get; }

        public LogSettings(Verbosity verbosity)
        {
            Verbosity = verbosity;
        }
    }
}